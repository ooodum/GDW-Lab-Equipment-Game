using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeUnit : Unit {
    Unit target;
    public override IEnumerator Attack() {
        if (!targetUnit) yield break;
        target = targetUnit;
        animator.SetTrigger("Sword");
        animator.CrossFade(ATTACK, .2f);
        yield return new WaitForSeconds(unit.Predelay);
        if (!targetUnit || targetUnit != target) yield break;
        FMODUnity.RuntimeManager.PlayOneShot("event:/Hit", transform.position);
        targetUnit.TakeDamage(unit.Damage);
        ClearUnit();
    }

    public override void Setup() {
    }
}
