using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class GameManager : MonoBehaviour {
    [SerializeField] TextMeshProUGUI friendlyText, enemyText;
    public static GameManager Instance { get; private set; }
    public PlayerInputActions inputActions;


    // Events
    public event Action<Vector3> MoveEvent;

    public event Action LockMouseEvent;
    public event Action LockMouseCancelledEvent;

    public event Action<Vector2> LookEvent;

    public event Action ClickEvent;
    public event Action StartGameEvent;

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
        // Movement Input Events
        inputActions.Movement.Move.performed += OnMove;
        inputActions.Movement.Look.performed += OnLook;
        inputActions.Movement.LockMouse.performed += OnLockMouse;
        inputActions.Movement.LockMouse.canceled += OnLockMouse;

        // Prep Input Events
        inputActions.Prep.Click.performed += OnClick;
        inputActions.Prep.StartGame.performed += OnStartGame;

        // Play Input Events
    }
    public GameState CurrentGameState { get; private set; }

    public void SetGameState(GameState state) {
        switch (state) {
            case GameState.Preparation:
                CurrentGameState = GameState.Preparation;
                inputActions.Movement.Enable();
                inputActions.Prep.Enable();
                inputActions.Play.Disable();
                break;
            case GameState.Play:
                CurrentGameState = GameState.Play;
                inputActions.Prep.Disable();
                inputActions.Movement.Enable();
                inputActions.Play.Enable();
                break;
        }
    }

    void OnMove(InputAction.CallbackContext ctx) {
        MoveEvent?.Invoke(ctx.ReadValue<Vector3>());
    }

    void OnLook(InputAction.CallbackContext ctx) {
        LookEvent?.Invoke(ctx.ReadValue<Vector2>());
    }

    void OnLockMouse(InputAction.CallbackContext ctx) {
        if (ctx.phase == InputActionPhase.Performed) LockMouseEvent?.Invoke();
        if (ctx.phase == InputActionPhase.Canceled) LockMouseCancelledEvent?.Invoke();
    }

    void OnClick(InputAction.CallbackContext ctx) {
        ClickEvent?.Invoke();
    }

    void OnStartGame(InputAction.CallbackContext ctx) {
        StartGameEvent?.Invoke();
    }

    private void Update() {
        friendlyText.text = FriendlyTroops.Count.ToString();
        enemyText.text = EnemyTroops.Count.ToString();
    }
}
