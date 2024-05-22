using System.Collections.Generic;
using UnityEngine;

public static class CustomTriggers {
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
        }

        return objects;
    }
}