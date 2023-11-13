using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    public PlayerInputActions inputActions;


    // Events
    public event Action<Vector3> MoveEvent;
    public event Action MoveCancelledEvent;

    public event Action<Vector2> LookEvent;
    public event Action LookCancelledEvent;

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(this); else Instance = this;
        inputActions = new PlayerInputActions();
    }
    public List<Unit> FriendlyTroops = new List<Unit>();
    public List<Unit> EnemyTroops = new List<Unit>();

    public enum GameState {
        Preparation,
        Play
    }
    private void OnEnable() {
        inputActions.Play.Move.performed += OnMove;
        inputActions.Preparation.Move.performed += OnMove;
        inputActions.Play.Look.performed += OnLook;
        inputActions.Preparation.Look.performed += OnLook;
    }
    public GameState CurrentGameState { get; private set; }

    public void SetGameState(GameState state) {
        switch (state) {
            case GameState.Preparation:
                CurrentGameState = GameState.Preparation;
                inputActions.Play.Disable();
                inputActions.Preparation.Enable();
                break;
            case GameState.Play:
                CurrentGameState = GameState.Play;
                inputActions.Preparation.Disable();
                inputActions.Play.Enable();
                break;
        }
    }

    public void OnMove(InputAction.CallbackContext ctx) {
        if (ctx.phase == InputActionPhase.Performed) MoveEvent?.Invoke(ctx.ReadValue<Vector3>());
        if (ctx.phase == InputActionPhase.Canceled) MoveCancelledEvent?.Invoke();
    }

    public void OnLook(InputAction.CallbackContext ctx) {
        print(ctx.phase);
        LookEvent?.Invoke(ctx.ReadValue<Vector2>());
    }
}
