using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HammerAbility : MonoBehaviour
{
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

    void Start() {
        rb = transform.GetComponent<Rigidbody>();
        orientation = transform.Find("Orientation");
        enemyLayer = LayerMask.GetMask("Enemy");
        groundLayer = LayerMask.GetMask("Ground");
    }

    IEnumerator ChargeHammerCoroutine()
    {
        yield return new WaitForSeconds(chargeTime);

        if (isCharging) isCharged = true;
    }

    public void ChargeHammer()
    {
        isCharging = true;
        StartCoroutine(ChargeHammerCoroutine());
    }

    public void ActivateHammerAbility()
    {
        if (isCharged)
        {
            // Check if impact area is colliding with either a ground layer or enemy layer
            if (Physics.CheckBox(impactArea.transform.position, impactArea.size * 0.5f, Quaternion.identity, enemyLayer)) {
                // Stop player y velocity whilst keeping the other velocity axes
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce((Vector3.up + orientation.forward * 3f * enemyBounceMultiplier) * hammerForce, ForceMode.Impulse);
                isCharged = false;
            } else if (Physics.CheckBox(impactArea.transform.position, impactArea.size * 0.5f, Quaternion.identity, groundLayer)) {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce((Vector3.up + orientation.forward * 3f) * hammerForce, ForceMode.Impulse);
                isCharged = false;
            }
        }

        isCharged = false;
        isCharging = false;
        StopCoroutine(ChargeHammerCoroutine());
    }

    void OnGUI() {
        GUILayout.TextArea($"Hammer charging: {isCharging}");
        GUILayout.TextArea($"Hammer charged: {isCharged}");
    }
}
