using System.Collections;
using UnityEngine;

namespace TEE.Player.Movement {
    public class PlayerDashing : MonoBehaviour {
        public                        UnityEngine.Camera cam;
        OLD_PlayerMovement                                   pm;
        Rigidbody                                        rb;

        [Header("Dashing"), SerializeField] float dashForce;
        [SerializeField]                    float dashDuration;
        [SerializeField]                    float dashFOV;
        [SerializeField, Range(0, -90)]     int   dashLowerLimit;
        [SerializeField, Range(0, 90)]      int   dashUpperLimit;
        float                                     originalFOV;
        Vector3                                   dashDirection;

        [Header("Cooldown"), SerializeField] float dashCooldown;
        float                                      dashCooldownTimer;

        void Start() {
            rb          = GetComponent<Rigidbody>();
            pm          = GetComponent<OLD_PlayerMovement>();
            originalFOV = cam.fieldOfView;
        }

        public void FixedUpdate() {
            if (dashCooldownTimer > 0) dashCooldownTimer -= Time.deltaTime;
            if (pm.dashing) DashingMovement();
        }

        public void Dash() {
            if (dashCooldownTimer > 0) return;
            dashCooldownTimer = dashCooldown;

            rb.useGravity = false;
            pm.dashing    = true;

            float camAngle = cam.transform.localEulerAngles.x;
            float xAngle   = camAngle is >= 0 and <= 90 ? -camAngle : (camAngle <= 360 ? 360 - camAngle : 0);
            float y        = Map(xAngle, -89, 89, dashLowerLimit, dashUpperLimit);
            dashDirection = Quaternion.AngleAxis(y, cam.transform.right) * cam.transform.forward;

            StartCoroutine(ResetDash());
            StartCoroutine(LerpFOV(dashFOV, 0.1f));
        }

        void DashingMovement() {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(dashDirection * dashForce, ForceMode.Impulse);
        }

        IEnumerator ResetDash() {
            yield return new WaitForSeconds(dashDuration);
            pm.dashing    = false;
            rb.useGravity = true;
            StartCoroutine(LerpFOV(originalFOV, 0.25f));
        }

        IEnumerator LerpFOV(float targetFOV, float duration) {
            float t           = 0;
            float startingFOV = cam.fieldOfView;

            while (t < duration) {
                float fov = Mathf.Lerp(startingFOV, targetFOV, t / duration);
                cam.fieldOfView =  fov;
                t               += Time.deltaTime;
                yield return null;
            }
        }

        float Map(float value, float inMin, float inMax, float outMin, float outMax) {
            return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
        }
    }
}