using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUnit : Troop {
    [SerializeField] bool isFriendly;
    private void OnEnable() {
        if (isFriendly) GameManager.Instance.FriendlyTroops.Add(transform); else GameManager.Instance.EnemyTroops.Add(transform);
    }
    private void Start() {
        friendly = isFriendly;
    }
    protected override void Attack() {
        throw new System.NotImplementedException();
    }

    protected override void Ability() {
        throw new System.NotImplementedException();
    }
}
