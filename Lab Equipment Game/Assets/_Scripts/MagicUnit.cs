using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MagicUnit : Unit {
    int pool = 3, poolIndex = 0;
    Transform[] projectiles;
    //bool canAttack = true;
    public override IEnumerator Attack() {
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(unit.Predelay);

        if (!targetUnit) yield break;

        projectiles[poolIndex].gameObject.SetActive(true);
        projectiles[poolIndex].DOMove(targetTransform.position, .2f).OnComplete(DamageEnemy);
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
        if (!targetUnit) return;
        FMODUnity.RuntimeManager.PlayOneShot("event:/Magic", transform.position);
        targetUnit.TakeDamage(unit.Damage);
        ClearUnit();
        projectiles[poolIndex].transform.position = transform.position;
        projectiles[poolIndex].gameObject.SetActive(false);
    }
}
