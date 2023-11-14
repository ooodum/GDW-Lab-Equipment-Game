using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeUnit : Unit {
    public override IEnumerator Attack() {
        animator.SetTrigger("Sword");
        animator.CrossFade(ATTACK, .2f);
        yield return new WaitForSeconds(unit.Predelay);
        targetUnit.TakeDamage(unit.Damage);
    }

    public override void Setup() {
    }
}
