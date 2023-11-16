using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class Unit : MonoBehaviour {
    public UnitInfo unit;
    protected Animator animator;
    Rigidbody rb;
    Collider col;
    protected State currentState;
    float attackTimer = 0, hp, distanceToTarget;
    protected Transform targetTransform;
    protected Unit targetUnit;
    Vector3 targetDirection;

    public readonly int ATTACK = Animator.StringToHash("Attack");
    public readonly int BOW = Animator.StringToHash("Bow");
    public readonly int IDLE = Animator.StringToHash("Idle");
    public readonly int SWORD = Animator.StringToHash("Sword");
    public readonly int WALK = Animator.StringToHash("Walk");

    [HideInInspector]
    public int currentAttackers; // Denotes how many enemy units are attacking this

    protected enum State {
        Idle,
        Run,
        Attack,
        Death
    }
    private void Awake() {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }
    private void Start() {
        if (unit.Friendly) GameManager.Instance.FriendlyTroops.Add(this); else GameManager.Instance.EnemyTroops.Add(this);

        hp = unit.Health;
        GameObject model = unit.Friendly ? Instantiate(unit.Model, transform.position + Vector3.up, Quaternion.identity, transform) : transform.GetChild(0).gameObject;
        animator = model.GetComponent<Animator>();
        SwitchState(State.Idle);

        Setup();

        attackTimer = 1f / unit.AttackSpeed;

    }

    void Update() {
        SetTarget();
        UpdateTarget();
        UpdateStates();
        UpdateRotations();
        TestUpdate();
        //print($"This: {transform.name}, Target: {targetTransform.name}");

    }

    protected void SwitchState(State state) {
        currentState = state;

        switch (state) {
            case State.Idle:
                animator.SetTrigger("Idle");
                break;
            case State.Run:
                if (targetUnit) animator.SetTrigger("Walk"); else animator.SetTrigger("Idle");
                break;
            default: break;
                //case State.Death: break;
        }
    }

    void UpdateStates() {
        if (attackTimer <= 1f / unit.AttackSpeed) {
            attackTimer += Time.deltaTime;
        }

            switch (currentState) {
            case State.Idle:
                if (GameManager.Instance.CurrentGameState == GameManager.GameState.Play) SwitchState(State.Run);
                break;

            case State.Run:
                rb.velocity = unit.MoveSpeed * targetDirection;
                if (InRangeToAttack()) SwitchState(State.Attack);
                break;

            case State.Attack:
                if (targetUnit == null) {
                    ClearUnit();
                    SwitchState(State.Run);
                }
                if (attackTimer >= 1f / unit.AttackSpeed) {
                    attackTimer = 0;
                    animator.CrossFade(currentState.ToString(), .1f);
                    StartCoroutine(Attack());
                }
                break;

            case State.Death:
                if (targetUnit) targetUnit.currentAttackers--;
                FMODUnity.RuntimeManager.PlayOneShot("event:/Death", transform.position);
                Destroy(gameObject);
                break;

            default: break;
        }
    }

    void UpdateRotations() {
        Quaternion currentRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.identity;

        Vector3 lookAt = Vector3.zero;

        if (rb.velocity.magnitude > 0) {
            lookAt = targetDirection;
            lookAt.y = 0;
            lookAt = lookAt.normalized;
        }

        if (rb.velocity.sqrMagnitude != 0 && lookAt != Vector3.zero) {
            targetRotation = Quaternion.LookRotation(lookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, 15 * Time.deltaTime);
        }
    }

    void UpdateTarget() {
        if (unit.Friendly) {
            if (GameManager.Instance.EnemyTroops.Count < 1) return;
        } else {
            if (GameManager.Instance.FriendlyTroops.Count < 1) return;
        }

        if (targetTransform != null) {
            Vector3 v = targetTransform.position - transform.position;
            distanceToTarget = v.magnitude;
            targetDirection = v.normalized;
        }
    }

    void SetTarget() {
        //if (targetTransform != null) return;
        if (unit.Friendly) {
            GetTargetTroop(GameManager.Instance.EnemyTroops);

        } else {
            GetTargetTroop(GameManager.Instance.FriendlyTroops);

            /*foreach (Unit unit in GameManager.Instance.FriendlyTroops) {
                Transform t = unit.transform;
                if (targetTransform != null) if (!((transform.position - t.position).magnitude < (transform.position - targetTransform.position).magnitude)) return;
                if (!(unit.currentAttackers < 3)) return;

                targetTransform = t;
                targetUnit = unit;
                unit.currentAttackers++;
            }*/
        }

    }

    void GetTargetTroop(List<Unit> units) {
        if (units == null || units.Count == 0 || InRangeToAttack()) return;
        foreach (Unit unit in units) {
            Transform t = unit.transform;
            if (targetTransform == null) {
                targetTransform = t;
                targetUnit = unit;
            }
            if (((transform.position - t.position).magnitude < (transform.position - targetTransform.position).magnitude)) {
                targetTransform = t;
                targetUnit = unit;
                print("new target found");
            }
        }
    }
    public void TakeDamage(float damage) {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Hurt", transform.position);
        hp -= damage;
        if (hp <= 0) {
            SwitchState(State.Death);
        }
    }
    private void OnDestroy() {
        if (unit.Friendly) GameManager.Instance.FriendlyTroops.Remove(this);
        else GameManager.Instance.EnemyTroops.Remove(this);
    }
    public abstract IEnumerator Attack();
    public abstract void Setup();

    bool InRangeToAttack() {
        if (distanceToTarget == 0) return false;
        return distanceToTarget <= Mathf.Max(unit.Range, 2 * col.bounds.extents.z + .03f);
    }

    protected void ClearUnit() {
        distanceToTarget = 0;
        targetTransform = null;
        targetUnit = null;
    }

    protected virtual void TestUpdate() { } //Used to test stuff inside inherited troops, don't use this to do game logic
}
