using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
    public UnitInfo currentlySelectedUnit = null;

    GameManager manager;
    PlayerInputActions inputActions;
    Rigidbody rb;
    Vector2 look;
    Vector3 moveInput, relativeMoveInput;
    bool mouseLock = false;
    [SerializeField] GameObject baseUnit;
    [SerializeField] LayerMask groundLayer, unitLayer;

    [SerializeField] float moveSpeed, sensitivity;


    [Header("Prep Stage")]
    [SerializeField] RectTransform coinBar, hintBar;
    [SerializeField] int cash;
    [SerializeField] TextMeshProUGUI coinText, errorText;
    [SerializeField] CanvasGroup errorCanvas;
    [SerializeField] SelectBar selectBar;


    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }
    void Start() {
        manager = GameManager.Instance;

        manager.player = this;

        manager.MoveEvent += _move;
        manager.LookEvent += Look;
        manager.LockMouseEvent += _lockmouse;
        manager.LockMouseCancelledEvent += _unlockmouse;
        manager.ClickEvent += Click;
        manager.StartGameEvent += StartGame;

        manager.SetGameState(GameManager.GameState.Preparation);

        hintBar.anchoredPosition = Vector3.left * 16;
        coinText.text = cash.ToString();

        errorCanvas.alpha = 0;

        //
    }

    void _move(Vector3 input) {
        moveInput = input;
        relativeMoveInput =
            moveInput.x * transform.right +
            moveInput.y * transform.up +
            moveInput.z * transform.forward;
    }

    void _lockmouse() {
        mouseLock = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void _unlockmouse() {
        mouseLock = false;
        Cursor.lockState = CursorLockMode.None;
    }

    private void OnDisable() {
        manager.MoveEvent -= _move;
        manager.LookEvent -= Look;
        manager.LockMouseEvent -= _lockmouse;  
        manager.ClickEvent -= Click;
        manager.StartGameEvent -= StartGame;
        manager.LockMouseCancelledEvent -= _unlockmouse;
    }

    void Update() {
        ProcessMovement();
        switch (manager.CurrentGameState) {
            case (GameManager.GameState.Preparation): break;
            case (GameManager.GameState.Play): break;
        }
    }

    void ProcessMovement() {
        rb.velocity = relativeMoveInput * moveSpeed;
    }

    void Look(Vector2 input) {
        if (!mouseLock) return;
        look.x = Mathf.Clamp(look.x - input.y * sensitivity, -90, 90);
        look.y += input.x * sensitivity;
        if (look.y >= 360 || look.y <= -360) look.y = 0;
        transform.localEulerAngles = Vector3.right * look.x + Vector3.up * look.y;
    }

    void Click() {
        if (currentlySelectedUnit == null || EventSystem.current.IsPointerOverGameObject()) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100)) {
            print(hit.transform.gameObject.layer);
            if (hit.transform.gameObject.layer == 3) {
                // Check to see if user has enough cash
                if (cash >= currentlySelectedUnit.Cost) {
                    cash -= currentlySelectedUnit.Cost;
                    coinText.text = cash.ToString();
                    DOTween.Kill(coinBar.GetInstanceID());
                    coinBar.anchoredPosition = new Vector2(-60, 45);
                    coinBar.DOPunchPosition(Vector3.up * 10, .4f).SetId(coinBar.GetInstanceID());
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Coin");
                } else {
                    ShowErrorText("Not enough money");
                    return;
                }

                // Place an object
                Unit unit = null;
                GameObject tempBaseUnit = Instantiate(baseUnit, hit.point + Vector3.up * .5f, currentlySelectedUnit.Model.transform.rotation);
                switch (currentlySelectedUnit.Type) {
                    case UnitInfo.UnitType.Melee:
                        unit = (Unit)tempBaseUnit.AddComponent(typeof(MeleeUnit));
                        unit.unit = currentlySelectedUnit;
                        break;
                    case UnitInfo.UnitType.Ranged:
                        unit = (Unit)tempBaseUnit.AddComponent(typeof(RangedUnit));
                        unit.unit = currentlySelectedUnit;
                        break;
                    case UnitInfo.UnitType.Magic:
                        unit = (Unit)tempBaseUnit.AddComponent(typeof(MagicUnit));
                        unit.unit = currentlySelectedUnit;
                        break;
                    default: break;
                }
            } else if (hit.transform.gameObject.layer == unitLayer) {

            }
        }
    }

    void StartGame() {
        if (manager.CurrentGameState != GameManager.GameState.Preparation) return;
        if (manager.FriendlyTroops.Count == 0) {
            ShowErrorText("No troops placed");
            return;
        }
        selectBar.Hide();
        hintBar.DOAnchorPosX(420, .6f).SetEase(Ease.InOutQuint);
        manager.SetGameState(GameManager.GameState.Play);
    }

    void ShowErrorText(string text) {
        errorText.text = text;
        errorCanvas.alpha = 1;

        DOTween.Kill(errorCanvas.GetInstanceID());

        errorCanvas.DOFade(0, .5f).SetDelay(1f).SetId(errorCanvas.GetInstanceID());
    }
}
