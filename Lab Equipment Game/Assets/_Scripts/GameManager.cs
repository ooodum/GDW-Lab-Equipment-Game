using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance {get; private set;}
    private void Awake() {
        if (Instance != null && Instance != this) Destroy(this); else Instance = this;
    }
    public List<Transform> FriendlyTroops = new List<Transform>();
    public List<Transform> EnemyTroops = new List<Transform>();
}
