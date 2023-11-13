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

        hp = unit.Health;

        var model = Instantiate(unit.Model, transform.position, Quaternion.identity, transform);
        animator = model.GetComponent<Animator>();
    }
    private void Start() {
        if (unit.Friendly) GameManager.Instance.FriendlyTroops.Add(this); else GameManager.Instance.EnemyTroops.Add(this);

        Setup();
    }

    void Update() {
        SetTarget();

        UpdateTarget();
        UpdateStates();
        PursueTarget();
        UpdateRotations();
        //print($"This: {transform.name}, Target: {targetTransform.name}");

    }

    protected void SwitchState(State state) {
        currentState = state;

        //animator.CrossFade(currentState.ToString(), .2f);
    }

    void UpdateStates() {
        print(currentState);
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
        if (unit.Friendly) {
            if (GameManager.Instance.EnemyTroops.Count < 1) return;
        } else {
            if (GameManager.Instance.FriendlyTroops.Count < 1) return;
        }

        if (targetTransform != null) {
            Vector3 v = transform.position - targetTransform.position;
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

    void PursueTarget() {
        rb.velocity = targetDirection * unit.MoveSpeed;
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
