using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RangedUnit : Unit {
    [Header("Attack")]

    int pool = 3, poolIndex = 0;
    Transform[] projectiles;
    Unit currentUnit;
    //bool canAttack = true;
    public override IEnumerator Attack() {
        animator.SetTrigger("Bow");
        yield return new WaitForSeconds(unit.Predelay);

        if (!targetUnit) yield break;

        projectiles[poolIndex].gameObject.SetActive(true);
        FMODUnity.RuntimeManager.PlayOneShot("event:/Bow Shoot", transform.position);
        currentUnit = targetUnit;
        projectiles[poolIndex].DOMove(targetTransform.position, .4f).OnComplete(DamageEnemy);
        poolIndex = poolIndex++ % pool;
    }

    public override void Setup() {
        projectiles = new Transform[pool];
        for (int i = 0; i < pool; i++) {
            GameObject temp = Instantiate(unit.Projectile, transform.position, Quaternion.identity, transform);
            projectiles[i] = temp.transform;
            temp.gameObject.SetActive(false);
        }
    }

    void DamageEnemy() {
        if (!targetUnit || currentUnit != targetUnit) return;
        targetUnit.TakeDamage(unit.Damage);
        ClearUnit();
        FMODUnity.RuntimeManager.PlayOneShot("event:/Bow Hit", transform.position);
        projectiles[poolIndex].gameObject.SetActive(false);
    }
}
