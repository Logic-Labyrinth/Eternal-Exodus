using UnityEngine;

public class PlayerDashing : MonoBehaviour {
    [Header("References")]
    public Transform orientation;
    PlayerMovement pm;
    Rigidbody rb;

    [Header("Dashing")]
    [SerializeField] float dashForce;
    [SerializeField] float dashDuration;

    [Header("Cooldown")]
    [SerializeField] float dashCooldown;
    float dashCDTimer;

    void Start() {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
    }

    void Update() {
        // if (Input.GetKeyDown(KeyCode.Mouse1)) Dash();
        if (dashCDTimer > 0) dashCDTimer -= Time.deltaTime;
    }

    Vector3 delayedForceToApply;
    public void Dash() {
        if (dashCDTimer > 0) return;
        else dashCDTimer = dashCooldown;

        delayedForceToApply = orientation.forward * dashForce;
        rb.useGravity = false;
        pm.dashing = true;
        Invoke(nameof(DelayedDashForce), 0.025f);
        Invoke(nameof(ResetDash), dashDuration);
    }

    void DelayedDashForce() {
        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    void ResetDash() {
        pm.dashing = false;
        rb.useGravity = true;
    }
}
