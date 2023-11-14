using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class Unit : MonoBehaviour {
    public UnitInfo unit;
    Animator animator;
    Rigidbody rb;
    Collider col;
    protected State currentState;
    float attackTimer = 0, hp, distanceToTarget;
    protected Transform targetTransform;
    protected Unit targetUnit;
    Vector3 targetDirection;

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
        SwitchState(State.Idle);   
    }
    private void Start() {
        if (unit.Friendly) GameManager.Instance.FriendlyTroops.Add(this); else GameManager.Instance.EnemyTroops.Add(this);

        hp = unit.Health;
        var model = Instantiate(unit.Model, transform.position + Vector3.up, Quaternion.identity, transform);
        animator = model.GetComponent<Animator>();

        Setup();
    }

    void Update() {
        SetTarget();

        UpdateTarget();
        UpdateStates();
        UpdateRotations();
        //print($"This: {transform.name}, Target: {targetTransform.name}");

    }

    protected void SwitchState(State state) {
        currentState = state;

        //animator.CrossFade(currentState.ToString(), .2f);
    }

    void UpdateStates() {
        switch (currentState) {
            case State.Idle:
                if (GameManager.Instance.CurrentGameState == GameManager.GameState.Play) SwitchState(State.Run);
                break;

            case State.Run:
                if (targetUnit == null) return;
                rb.velocity = unit.MoveSpeed * targetDirection;
                if (distanceToTarget <= Mathf.Max(unit.Range, 2 * col.bounds.extents.z + .03f)) {
                    SwitchState(State.Attack);
                }
                break;

            case State.Attack:
                if (attackTimer <= 1f / unit.AttackSpeed) {
                    attackTimer += Time.deltaTime;
                } else {
                    print($"ATTACKING {targetUnit.unit.Friendly}");
                    attackTimer = 0;
                    animator.CrossFade(currentState.ToString(), .1f);
                    StartCoroutine(Attack());
                }
                break;

            case State.Death:
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
            lookAt = rb.velocity;
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
            Vector3 v = targetTransform.position- transform.position;
            distanceToTarget = v.magnitude;
            targetDirection = v.normalized;
        }
    }

    void SetTarget() {
        if (targetTransform != null) return;
        if (unit.Friendly) {
            targetTransform = null;
            foreach (Unit unit in GameManager.Instance.EnemyTroops) {
                Transform t = unit.transform;
                if (targetTransform != null) if (!((transform.position - t.position).magnitude < (transform.position - targetTransform.position).magnitude)) return;
                if (!(unit.currentAttackers < 3)) return;

                targetTransform = t;
                targetUnit = unit;
                unit.currentAttackers++;
            }
        } else {
            targetTransform = null;
            foreach (Unit unit in GameManager.Instance.FriendlyTroops) {
                Transform t = unit.transform;
                if (targetTransform != null) if (!((transform.position - t.position).magnitude < (transform.position - targetTransform.position).magnitude)) return;
                if (!(unit.currentAttackers < 3)) return;

                targetTransform = t;
                targetUnit = unit;
                unit.currentAttackers++;
            }
        }

    }

    public void TakeDamage(float damage) {
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
}
