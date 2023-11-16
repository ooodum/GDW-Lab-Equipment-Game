using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    [SerializeField] TextMeshProUGUI friendlyText, enemyText;
    [SerializeField] CanvasGroup loadingText, winScreen, loseScreen, pauseScreen;
    [SerializeField] RectTransform bg, loadingBox;
    [SerializeField] int level;
    public static GameManager Instance { get; private set; }
    public PlayerInputActions inputActions;

    public PlayerController player;
    bool screenTriggered = false;

    // Events
    public event Action<Vector3> MoveEvent;

    public event Action LockMouseEvent;
    public event Action LockMouseCancelledEvent;

    public event Action<Vector2> LookEvent;

    public event Action ClickEvent;
    public event Action StartGameEvent;

    public List<Unit> FriendlyTroops = new List<Unit>();
    public List<Unit> EnemyTroops = new List<Unit>();

    public static bool Paused;

    [Header("Music")]
    [SerializeField] FMOD.Studio.EventInstance menuMusic, gameMusic;

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(this); else Instance = this;
        inputActions = new PlayerInputActions();
    }
    private void Start() {
        menuMusic = FMODUnity.RuntimeManager.CreateInstance("event:/Main Theme");
        gameMusic = FMODUnity.RuntimeManager.CreateInstance("event:/Fight Theme");


        menuMusic.start();

        bg.anchoredPosition = Vector3.left * 2171;

        loadingBox.sizeDelta = new Vector3(0, 100);
        loadingText.alpha = 0;
    }


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
                menuMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                gameMusic.start();
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

        if (CurrentGameState == GameState.Play) {
            if (screenTriggered) return;
            if (EnemyTroops.Count == 0) {
                screenTriggered = true;
                winScreen.DOFade(1, 1f);
                winScreen.interactable = true;
                winScreen.blocksRaycasts = true;
            } else if (FriendlyTroops.Count == 0) {
                screenTriggered = true;
                loseScreen.DOFade(1, 1f);
                loseScreen.interactable = true;
                loseScreen.blocksRaycasts = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Paused = !Paused;

            if (Paused) {
                Time.timeScale = 0;
                pauseScreen.alpha = 1;
                pauseScreen.blocksRaycasts = true;
                
            } else {
                Time.timeScale = 1;
                pauseScreen.alpha = 0;
                pauseScreen.blocksRaycasts = false;
            }
        }
    }

    public void NextLevel(RectTransform button) {
        ButtonClick(button, string.Concat("Level", level + 1));
    }

    public void GoHome(RectTransform button) {
        ButtonClick(button, "Main Menu");
    }

    public void Retry(RectTransform button) {
        ButtonClick(button, string.Concat("Level", level));
    }

    public void ButtonClick(RectTransform button, string level) {
        button.GetComponent<UIButton>().interactable = false;
        button.DOSizeDelta(new Vector2(2000, 100), .8f).SetEase(Ease.OutCirc).SetUpdate(true);
        bg.DOAnchorPosX(0, .6f).SetEase(Ease.OutCirc);

        loadingBox.DOSizeDelta(new Vector2(350, 120), .7f).SetUpdate(true).SetEase(Ease.OutBack).SetDelay(1).OnComplete(() => {
            loadingText.DOFade(1, .8f).SetDelay(.2f).SetUpdate(true);
            LoadScene(level);
        });
    }

    async void LoadScene(string level) {
        menuMusic.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        gameMusic.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        await Awaitable.WaitForSecondsAsync(2);
        SceneManager.LoadScene(level);
    }
}
