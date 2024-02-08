using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class PlayerMovement : MonoBehaviour {
    public float walkingSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    bool hasDoubleJumped = false;

    void Start() {
        characterController = GetComponent<CharacterController>();
    }

    void Update() {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = walkingSpeed * Input.GetAxis("Vertical");
        float curSpeedY = walkingSpeed * Input.GetAxis("Horizontal");
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        HandleJumping(movementDirectionY);

        characterController.Move(moveDirection * Time.deltaTime);
    }

    void HandleJumping(float movementDirectionY) {
        if (Input.GetButtonDown("Jump")) {
            if (characterController.isGrounded) {
                // Jump
                moveDirection.y = jumpSpeed;
                hasDoubleJumped = false;
            } else {
                if (!hasDoubleJumped) {
                    // Double Jump
                    hasDoubleJumped = true;
                    moveDirection.y = jumpSpeed;
                } else moveDirection.y = movementDirectionY;
            }
        } else moveDirection.y = movementDirectionY;

        if (!characterController.isGrounded) {
            moveDirection.y -= gravity * Time.deltaTime;
        }
    }
}
