using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 
public class FirstPersonController : MonoBehaviour
{
    public bool CanMove { get; private set; } = true;

    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintModifier = 1.7f;
    [SerializeField] private float gravity = 20f;
    [SerializeField] private float jumpHeight = 20f;

    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 180)] private float upperLookLimit = 80.0f;
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 80.0f;

    public float health = 100f;
    public float stamina = 100f;

    private Camera playerCamera;
    private CharacterController characterController;

    private Vector3 moveDirection;
    private Vector2 currentInput;
    [SerializeField] UIController ui;

    private float rotationX = 0;

   
    void Awake() 
    {
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update() 
    {
        if (health <= 0f) {
            Death();
        }
        if (CanMove) {
            HandleMovementInput();
            HandleMouseLook();
            stamina += 0.5f;
            stamina = Mathf.Clamp(stamina, 0f, 100f) ;
            ApplyFinalMovements();
        }
    }
    private void HandleMovementInput() 
    {
        currentInput = new Vector2(walkSpeed * Input.GetAxis("Vertical"), walkSpeed * Input.GetAxis("Horizontal"));

        if (Input.GetButton("Sprint")){
            if (stamina > 0f) {
                currentInput *= sprintModifier;
                stamina -= 0.5f;
            }
        }

        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY;
        if (Input.GetButton("Jump")){
            Jump();
        }
    }
    private void HandleMouseLook() 
    {
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);
    }
    private void ApplyFinalMovements() 
    { 
        if (!characterController.isGrounded) { //TODO:

        }
            moveDirection.y -= gravity * Time.deltaTime;

        characterController.Move(moveDirection * Time.deltaTime);
    }
    private void Jump(){ //TODO:
        moveDirection.z += jumpHeight * Time.deltaTime;
    }
    public void HealthChange(float damage) {
        health += damage;
        Mathf.Clamp(health, 0f, 150f);
    }

    void Death() {
        ui.DeathScreen();
    }
}
