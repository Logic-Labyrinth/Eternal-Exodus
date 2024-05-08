using System;
using System.Collections.Generic;
using UnityEngine;

public static class CustomConeCollider {
    // Black magic :)
    public static List<GameObject> GetAllObjects(Transform t, float radius, float vAngle, float hAngle) {
        Collider[] colliders = Physics.OverlapSphere(t.position, radius, -1, QueryTriggerInteraction.Ignore);
        List<GameObject> objects = new();

        foreach (Collider col in colliders) {
            Vector3 dir = col.transform.position - t.position;
            // float verticalAngle = Vector3.Angle(dir, t.up);

            Vector3 proj = Vector3.ProjectOnPlane(dir, t.up);
            float horizontalAngle = Vector3.SignedAngle(proj, t.forward, t.up);

            // Debug.Log("Name: " + col.name + " Horizontal: " + horizontalAngle);

            if (Math.Abs(horizontalAngle) < hAngle / 2)
                objects.Add(col.gameObject);
        }

        if(objects.Count > 0) return objects;
        return null;
    }
}