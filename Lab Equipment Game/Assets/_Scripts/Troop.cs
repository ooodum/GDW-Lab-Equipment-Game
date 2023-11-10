using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody))]
public abstract class Troop : MonoBehaviour {
    protected Animator animator;
    protected Rigidbody rb;
    [SerializeField] TroopInfo troopInfo;
    protected int hp, damage;
    protected float range, moveSpeed;
    protected bool friendly;
    UnitState currentState;
    private void Awake() {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        hp = troopInfo.hp;
        damage = troopInfo.damage;
        range = troopInfo.range;
        moveSpeed = troopInfo.moveSpeed;
    }
    protected abstract void Attack();
    protected abstract void Ability();
    protected virtual void Die() {

    }
    public virtual void TakeDamage(int damage) {
        hp -= damage;
        if (hp <= 0) Die();
    }
    private void FixedUpdate() {
        foreach (Transform t in friendly ? GameManager.Instance.EnemyTroops : GameManager.Instance.FriendlyTroops) {

        }
    }
}
