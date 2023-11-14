using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "Unit/New Unit")]
public class UnitInfo : ScriptableObject {
    [Header("Stats")]
    public float Damage;
    public float Health;
    public float Range;
    public float Cost;
    public float AttackSpeed;
    public float MoveSpeed;
    public float Predelay;

    [Header("Info")]
    public string Name;
    public GameObject Model;
    public bool Friendly;
    public UnitType Type;

    [Header("Attack")]
    public Transform Projectile;
    public ParticleSystem particles;

    public enum UnitType {
        Melee,
        Ranged,
        Magic
    }
}
