using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RangedUnit : Unit {
    [Header("Attack")]

    int pool = 3, poolIndex = 0;
    Transform[] projectiles;

    public override IEnumerator Attack() {
        animator.SetTrigger("Bow");
        yield return new WaitForSeconds(unit.Predelay);

        projectiles[poolIndex].gameObject.SetActive(true);
        projectiles[poolIndex].DOMove(targetTransform.position, .4f).OnComplete(() => { 
            targetUnit.TakeDamage(unit.Damage);
            projectiles[poolIndex].gameObject.SetActive(false);
        });
        poolIndex = poolIndex++ % pool;
    }

    public override void Setup() {
        projectiles = new Transform[pool];
        for (int i = 0; i < pool; i++) {
            Transform temp = Instantiate(unit.Projectile, transform.position, Quaternion.identity, transform);
            projectiles[i] = temp;
            temp.gameObject.SetActive(false);
        }
    }
}
