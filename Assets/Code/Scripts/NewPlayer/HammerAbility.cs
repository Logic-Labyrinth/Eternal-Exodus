using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HammerAbility : MonoBehaviour {
    [SerializeField]
    private float chargeTime = 1f;
    [SerializeField]
    private BoxCollider impactArea;
    [SerializeField]
    private float hammerForce;
    [SerializeField]
    private float enemyBounceMultiplier;
    private LayerMask enemyLayer;
    private LayerMask groundLayer;
    private bool isCharging = false;
    private bool isCharged = false;
    private Rigidbody rb;
    private Transform orientation;

    public Slider slider;
    public Image handle, background;
    float timer = 0;

    void Start() {
        rb = transform.GetComponent<Rigidbody>();
        orientation = transform.Find("Orientation");
        enemyLayer = LayerMask.GetMask("Enemy");
        groundLayer = LayerMask.GetMask("Ground");
    }

    void Stuff() {
        if (isCharging) isCharged = true;
    }

    public void ChargeHammer() {
        isCharging = true;
        Invoke("Stuff", chargeTime);
        timer = 0;
        handle.color = Color.white;
        background.color = Color.white;
        slider.value = 0;
        slider.gameObject.SetActive(true);
    }

    private void Update() {
        if (isCharging) {
            if (timer <= chargeTime) {
                slider.value = timer / chargeTime;
                timer += Time.deltaTime;
            } else {
                slider.value = 1;
                handle.color = Color.green;
                background.color = Color.green;
            }
        }
    }

    public void ActivateHammerAbility() {
        if (isCharged) {
            // Check if impact area is colliding with either a ground layer or enemy layer
            if (Physics.CheckBox(impactArea.transform.position, impactArea.size * 0.5f, Quaternion.identity, enemyLayer)) {
                // Stop player y velocity whilst keeping the other velocity axes
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce((Vector3.up + 3f * enemyBounceMultiplier * orientation.forward) * hammerForce, ForceMode.Impulse);
                isCharged = false;
            } else if (Physics.CheckBox(impactArea.transform.position, impactArea.size * 0.5f, Quaternion.identity, groundLayer)) {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce((Vector3.up + orientation.forward * 3f) * hammerForce, ForceMode.Impulse);
                isCharged = false;
            }
        }

        isCharged = false;
        isCharging = false;
        slider.gameObject.SetActive(false);
    }
}
