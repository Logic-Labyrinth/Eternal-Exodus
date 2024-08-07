using TEE.Input;
using UnityEngine;

namespace TEE.Player.Camera {
    public class ArmBobbing : MonoBehaviour {
        // Sway
        const float Step            = 0.01f;
        const float MaxStepDistance = 0.06f;
        const float RotationStep    = 4f;
        const float MaxRotationStep = 5;
        const float Smooth          = 10f;
        const float SmoothRot       = 12f;
        Vector3     swayPos;
        Vector3     swayEulerRot;
        Vector2     lookInput;
        float       rotationX;
        float       rotationY;
        Rigidbody   rb;

        // Bobbing
        float speedCurve;
        float CurveSin => Mathf.Sin(speedCurve);
        float CurveCos => Mathf.Cos(speedCurve);

        [SerializeField] Vector3 travelLimit = Vector3.one * 0.025f;
        [SerializeField] Vector3 bobLimit    = Vector3.one * 0.01f;
        [SerializeField] Vector2 walkInput;
        [SerializeField] Vector3 multiplier;
        Vector3                  bobEulerRotation;
        Vector3                  bobPosition;


        void Start() {
            rb        = Movement.Player.Rigidbody;
            lookInput = InputManager.GetLookInput();
        }

        void Update() {
            // GetMouseInput();
            // GetKeyInput();
            Sway();
            SwayRotation();

            BobOffset();
            BobRotation();

            CompositePosRot();
        }

        void Sway() {
            Vector3 invertLook = lookInput * -Step;
            invertLook.x = Mathf.Clamp(invertLook.x, -MaxStepDistance, MaxStepDistance);
            invertLook.y = Mathf.Clamp(invertLook.y, -MaxStepDistance, MaxStepDistance);

            swayPos = invertLook;
        }

        void SwayRotation() {
            Vector2 invertLook = lookInput * -RotationStep;
            invertLook.x = Mathf.Clamp(invertLook.x, -MaxRotationStep, MaxRotationStep);
            invertLook.y = Mathf.Clamp(invertLook.y, -MaxRotationStep, MaxRotationStep);

            swayEulerRot = new Vector3(invertLook.y, invertLook.x, invertLook.x);
        }

        void CompositePosRot() {
            transform.localPosition = Vector3.Lerp(transform.localPosition, swayPos + bobPosition, Time.deltaTime * Smooth);

            transform.localRotation = Quaternion.Slerp(
                transform.localRotation,
                Quaternion.Euler(swayEulerRot) * Quaternion.Euler(bobEulerRotation),
                Time.deltaTime                 * SmoothRot
            );
        }

        void BobOffset() {
            speedCurve    += Time.deltaTime * (Movement.Player.IsGrounded ? rb.velocity.magnitude : 1f) + 0.01f;
            bobPosition.x =  CurveCos       * bobLimit.x * (Movement.Player.IsGrounded ? 1 : 0)         - walkInput.x * travelLimit.x;

            float yVal = CurveSin * bobLimit.y - rb.velocity.y * travelLimit.y;

            bobPosition.y = Mathf.Clamp(yVal, -0.1f, 0.1f);
            bobPosition.z = -(walkInput.y * travelLimit.z);
        }


        void BobRotation() {
            bobEulerRotation.x = walkInput != Vector2.zero ? multiplier.x * Mathf.Sin(2 * speedCurve) : multiplier.x * (Mathf.Sin(2 * speedCurve) / 2);
            bobEulerRotation.y = walkInput != Vector2.zero ? multiplier.y * CurveCos : 0;
            bobEulerRotation.z = walkInput != Vector2.zero ? multiplier.z * CurveCos * walkInput.x : 0;
        }

        // void GetMouseInput() {
        // lookInput.x = Input.GetAxisRaw("Mouse X") + Input.GetAxisRaw("Controller X");
        // lookInput.y = Input.GetAxisRaw("Mouse Y") + Input.GetAxisRaw("Controller Y");
        // }

        // void GetKeyInput() {
        // walkInput.x = Input.GetAxisRaw("Horizontal") + Input.GetAxisRaw("Horizontal Controller");
        // walkInput.y = Input.GetAxisRaw("Vertical")   + Input.GetAxisRaw("Vertical Controller");
        // }
    }
}