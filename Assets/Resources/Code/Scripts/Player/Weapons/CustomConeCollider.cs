using System;
using System.Collections.Generic;
using UnityEngine;

public static class CustomConeCollider {
    [Obsolete("GetAllObjects is deprecated, please use Raycast instead.")]
    public static List<GameObject> GetAllObjects(Transform t, float radius, float vAngle, float hAngle) {
        Collider[] colliders = Physics.OverlapSphere(t.position, radius, -1, QueryTriggerInteraction.Ignore);
        List<GameObject> objects = new();

        foreach (Collider col in colliders) {
            Vector3 dir = col.transform.position - t.position;
            // float verticalAngle = Vector3.Angle(dir, t.up);

            Vector3 proj = Vector3.ProjectOnPlane(dir, t.up);
            float horizontalAngle = Vector3.SignedAngle(proj, t.forward, t.up);

            // Debug.Log("Name: " + col.name + " Horizontal: " + horizontalAngle);

            if (Math.Abs(horizontalAngle) < hAngle / 2) {
                objects.Add(col.gameObject);
                Debug.DrawLine(t.position, col.transform.position, Color.red);
            } else
                Debug.DrawLine(t.position, col.transform.position, Color.blue);
        }
        Debug.Break();
        if (objects.Count > 0) return objects;
        return null;
    }

    public static List<GameObject> ConeRaycast(Transform t, float angle, float distance, int rayCount) {
        List<GameObject> objects = new();
        float halfAngle = angle / 2;
        float radius = Mathf.Tan(halfAngle * Mathf.Deg2Rad) * distance;
        Quaternion rotation = Quaternion.LookRotation(t.forward);

        for (int i = 0; i < rayCount; i++) {
            float theta = 2 * Mathf.PI / rayCount * i;

            Vector3 localPoint = new(Mathf.Cos(theta) * radius, Mathf.Sin(theta) * radius, 0);
            Vector3 worldPoint = t.position + rotation * (Vector3.forward * distance + localPoint);
            Vector3 rayDirection = (worldPoint - t.position).normalized;

            RaycastHit[] hits = Physics.RaycastAll(t.position, rayDirection, distance);
            foreach (RaycastHit hit in hits)
                if (!objects.Contains(hit.transform.gameObject)) objects.Add(hit.transform.gameObject);
        }

        return objects;
    }

    public static List<GameObject> ArcRaycast(Transform t, float angle, float distance, int rayCount) {
        List<GameObject> objects = new();
        float halfAngle = angle / 2;

        for (int i = 0; i < rayCount; i++) {
            float theta = Mathf.Lerp(-halfAngle, halfAngle, (float)i / (rayCount - 1));
            Quaternion rotation = Quaternion.AngleAxis(theta, t.up);
            Vector3 rayDirection = rotation * t.forward;
            RaycastHit[] hits = Physics.RaycastAll(t.position, rayDirection, distance);
            foreach (RaycastHit hit in hits)
                if (!objects.Contains(hit.transform.gameObject)) objects.Add(hit.transform.gameObject);

            // Debug.DrawLine(t.position, t.position + rayDirection * distance, Color.red);
        }

        // Debug.Break();
        return objects;
    }
}