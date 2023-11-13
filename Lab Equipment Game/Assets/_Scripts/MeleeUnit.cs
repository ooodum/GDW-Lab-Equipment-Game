using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeUnit : Unit {
    [Header("Attack")]
    [SerializeField] float predelay; // Time in seconds before the swing actually occurs in the animation

    public override IEnumerator Attack() {
        yield return new WaitForSeconds(predelay);
        targetUnit.TakeDamage(unit.Damage);
    }

    public override void Setup() {
    }
}
