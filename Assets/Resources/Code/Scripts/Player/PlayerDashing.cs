using System;
using System.Collections;
using UnityEngine;

public class PlayerDashing : MonoBehaviour {
    [Header("References")]
    public Transform orientation;
    public Camera cam;
    PlayerMovement pm;
    Rigidbody rb;

    [Header("Dashing")]
    [SerializeField] float dashForce;
    [SerializeField] float dashDuration;
    [SerializeField] float dashFOV;
    [SerializeField, Range(0, -90)] int dashLowerLimit;
    [SerializeField, Range(0, 90)] int dashUpperLimit;
    float originalFOV;
    Vector3 dashDirection;

    [Header("Cooldown")]
    [SerializeField] float dashCooldown;
    float dashCDTimer;

    void Start() {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
        originalFOV = cam.fieldOfView;
    }

    public void FixedUpdate() {
        if (dashCDTimer > 0) dashCDTimer -= Time.deltaTime;
        if (pm.dashing) DashingMovement();
    }

    public void Dash() {
        if (dashCDTimer > 0) return;
        else dashCDTimer = dashCooldown;

        rb.useGravity = false;
        pm.dashing = true;

        float camAngle = cam.transform.localEulerAngles.x;
        float XAngle = camAngle >= 0 && camAngle <= 90 ? -camAngle : (camAngle <= 360 ? 360 - camAngle : 0);
        float Y = Map(XAngle, -89, 89, dashLowerLimit, dashUpperLimit);
        dashDirection = Quaternion.AngleAxis(Y, cam.transform.right) * cam.transform.forward;

        StartCoroutine(ResetDash());
        StartCoroutine(LerpFOV(dashFOV, 0.1f));
    }

    void DashingMovement() {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(dashDirection * dashForce, ForceMode.Impulse);
    }

    IEnumerator ResetDash() {
        yield return new WaitForSeconds(dashDuration);
        pm.dashing = false;
        rb.useGravity = true;
        StartCoroutine(LerpFOV(originalFOV, 0.25f));
    }

    IEnumerator LerpFOV(float targetFOV, float duration) {
        float t = 0;
        float startingFOV = cam.fieldOfView;

        while (t < duration) {
            float fov = Mathf.Lerp(startingFOV, targetFOV, t / duration);
            cam.fieldOfView = fov;
            t += Time.deltaTime;
            yield return null;
        }
    }

    float Map(float value, float inMin, float inMax, float outMin, float outMax) {
        return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }
}
