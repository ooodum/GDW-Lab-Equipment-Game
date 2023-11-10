using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Troop", menuName = "ScriptableObjects/TroopInfo")]
public class TroopInfo : ScriptableObject {
    public float range, moveSpeed;
    public int hp, damage;
    public TroopType type;
    public enum TroopType {
        Melee,
        Ranged,
        Magic
    }
}
