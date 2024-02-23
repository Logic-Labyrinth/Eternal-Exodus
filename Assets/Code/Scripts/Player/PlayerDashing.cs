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

    [Header("Cooldown")]
    [SerializeField] float dashCooldown;
    float dashCDTimer;

    void Start() {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
        originalFOV = cam.fieldOfView;
    }

    void Update() {
        if (dashCDTimer > 0) dashCDTimer -= Time.deltaTime;
    }

    public void FixedUpdate() {
        if (pm.dashing) DashingMovement();
    }

    Vector3 delayedForceToApply;
    public void Dash() {
        if (dashCDTimer > 0) return;
        else dashCDTimer = dashCooldown;

        rb.useGravity = false;
        pm.dashing = true;
        Invoke(nameof(ResetDash), dashDuration);
        StartCoroutine(LerpFOV(dashFOV, 0.1f));
    }

    void DashingMovement() {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        
        float Y = Map(cam.transform.forward.y, -1, 1, dashLowerLimit / 90, dashUpperLimit / 90);
        Vector3 direction = new(cam.transform.forward.x, Y, cam.transform.forward.z);
        rb.AddForce(direction * dashForce, ForceMode.Impulse);
    }

    void DelayedDashForce() {
        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    void ResetDash() {
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
