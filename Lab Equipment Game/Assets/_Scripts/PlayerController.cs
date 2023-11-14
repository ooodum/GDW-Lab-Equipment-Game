using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
    public UnitInfo currentlySelectedUnit;

    GameManager manager;
    PlayerInputActions inputActions;
    Rigidbody rb;
    Vector2 look;
    Vector3 moveInput, relativeMoveInput;
    bool mouseLock = false;
    [SerializeField] GameObject baseUnit;
    [SerializeField] LayerMask groundLayer, unitLayer;

    [SerializeField] float moveSpeed, sensitivity;
    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }
    void Start() {
        manager = GameManager.Instance;

        manager.MoveEvent += (input) => {
            moveInput = input;
            relativeMoveInput =
                moveInput.x * transform.right +
                moveInput.y * transform.up +
                moveInput.z * transform.forward;
        };
        manager.LookEvent += Look;
        manager.LockMouseEvent += () => { 
            mouseLock = true;
            Cursor.lockState = CursorLockMode.Locked;
        };
        manager.LockMouseCancelledEvent += () => { 
            mouseLock = false;
            Cursor.lockState = CursorLockMode.None;
        };
        manager.ClickEvent += Click;
        manager.StartGameEvent += StartGame;

        manager.SetGameState(GameManager.GameState.Preparation);
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
        look.y = Mathf.Clamp(look.y + input.x * sensitivity, -360, 360);
        transform.localEulerAngles = Vector3.right * look.x + Vector3.up * look.y;
    }

    void Click() {
        if (currentlySelectedUnit == null) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100)) {
            //print(hit.transform.gameObject.layer);
            if (hit.transform.gameObject.layer == 3) {
                Unit unit = null;
                GameObject tempBaseUnit = Instantiate(baseUnit, hit.point + Vector3.up * .5f, Quaternion.identity);
                switch (currentlySelectedUnit.Type) {
                    case UnitInfo.UnitType.Melee:
                        unit = (Unit)tempBaseUnit.AddComponent(typeof(MeleeUnit));
                        unit.unit = currentlySelectedUnit;
                        break;
                    case UnitInfo.UnitType.Ranged:
                        unit = (Unit)tempBaseUnit.AddComponent(typeof(RangedUnit));
                        break;
                    case UnitInfo.UnitType.Magic:
                        unit = (Unit)tempBaseUnit.AddComponent(typeof(MagicUnit));
                        break;
                    default: break;
                }
            } else if (hit.transform.gameObject.layer == unitLayer) {

            }
        }
    }

    void StartGame() {
        if (manager.CurrentGameState != GameManager.GameState.Preparation) return;
        manager.SetGameState(GameManager.GameState.Play);
    }
}
