using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
    GameManager manager;
    PlayerInputActions inputActions;
    Rigidbody rb;
    Vector2 look;
    Vector3 moveInput, relativeMoveInput;

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
        manager.MoveCancelledEvent += () => { moveInput = Vector3.zero; };
        manager.LookEvent += Look;

        Cursor.lockState = CursorLockMode.Locked;
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
        look.x = Mathf.Clamp(look.x - input.y * sensitivity, -90, 90);
        look.y = Mathf.Clamp(look.y + input.x * sensitivity, -360, 360);
        transform.localEulerAngles = Vector3.right * look.x + Vector3.up * look.y;
    }
}
