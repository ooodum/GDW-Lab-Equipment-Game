using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class Unit : MonoBehaviour {
    [SerializeField] protected UnitInfo unit;
    Animator animator;
    Rigidbody rb;
    Collider col;
    protected State currentState;
    float attackTimer = 0, hp, distanceToTarget;
    protected Transform targetTransform;
    protected Unit targetUnit;
    Vector3 targetDirection;
    public int currentAttackers; // Denotes how many enemy units are attacking this

    protected enum State {
        Idle,
        Run,
        Attack,
        Death
    }
    private void Awake() {
        animator = transform.GetChild(0).GetComponent<Animator>();
        col = GetComponent<Collider>();
        SwitchState(State.Idle);

        hp = unit.Health;
    }
    private void Start() {
        if (unit.Friendly) GameManager.Instance.FriendlyTroops.Add(this); else GameManager.Instance.EnemyTroops.Add(this);
        Setup();
        SetTarget();
    }

    void Update() {
        UpdateTarget();
        UpdateStates();
        UpdateRotations();
    }

    protected void SwitchState(State state) {
        currentState = state;

        animator.CrossFade(currentState.ToString(), .2f);
    }

    void UpdateStates() {
        switch (currentState) {
            case State.Idle: break;

            case State.Run:
                rb.velocity = unit.MoveSpeed * targetDirection;

                if (distanceToTarget <= unit.Range) {
                    SwitchState(State.Attack);
                }
                break;

            case State.Attack:
                if (attackTimer <= 1f / unit.AttackSpeed) {
                    attackTimer += Time.deltaTime;
                } else {
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
        if (targetTransform == null) {
            Vector3 v = targetTransform.position - transform.position;
            distanceToTarget = v.magnitude;
            targetDirection = v.normalized;
        }
    }

    void SetTarget() {
        if (unit.Friendly) {
            targetTransform = null;
            foreach (Unit unit in GameManager.Instance.EnemyTroops) {
                Transform t = unit.transform;
                if (!((transform.position - t.position).magnitude < (transform.position - targetTransform.position).magnitude)) return;
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

    public abstract IEnumerator Attack();
    public abstract void Setup();
}
