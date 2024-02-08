using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class Dash : MonoBehaviour {
    public float dashDistance;
    public float dashDuration;

    CharacterController controller;
    Vector3 dashEndLocation;
    float dashHitDistance, dashDurationScaled;

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(dashEndLocation, 1);
    }

    void Start() {
        controller = GetComponent<CharacterController>();
        dashEndLocation = Vector3.zero;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.LeftShift)) StartDash();
    }

    void calculateDashEndLocation() {
        dashHitDistance = dashDistance;

        RaycastHit hit;
        Vector3 p1 = transform.position + controller.center + Vector3.up * -controller.height * 0.5f;
        Vector3 p2 = p1 + Vector3.up * controller.height;

        if (Physics.CapsuleCast(p1, p2, controller.radius * 0.9f, transform.forward, out hit, dashDistance)) {
            if (hit.distance > 0.3) {
                Debug.Log("Hit! Old: " + dashDistance + " New: " + hit.distance);
                dashHitDistance = hit.distance;
            }
        }

        dashDurationScaled = dashDuration * (dashHitDistance / dashDistance);
        dashEndLocation = transform.position + transform.forward * dashHitDistance;
    }

    void StartDash() {
        calculateDashEndLocation();
        if (dashHitDistance == 0) return;
        Debug.DrawLine(transform.position, dashEndLocation, Color.red, 10);
        StartCoroutine(DashToPosition());
    }

    IEnumerator DashToPosition() {
        float dashTimer = 0;
        Vector3 startPos = transform.position;

        while (dashTimer < dashDurationScaled) {
            float t = dashTimer / dashDurationScaled;
            // dashEndLocation.y = transform.position.y;
            transform.position = Vector3.Lerp(startPos, dashEndLocation, t);
            dashTimer += Time.deltaTime;
            yield return null;
        }

        transform.position = dashEndLocation;
    }
}
