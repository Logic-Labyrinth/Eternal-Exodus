using System.Collections.Generic;
using UnityEngine;

public static class CustomCapsuleCollider {
    // It's just a chonky RaycastAll really...
    public static List<GameObject> GetAllObjects(Transform t, Vector3 direction, float radius, float distance) {
        RaycastHit[] hits = Physics.SphereCastAll(t.position, radius, direction, distance);

        List<GameObject> objects = new();
        foreach (RaycastHit hit in hits) objects.Add(hit.transform.gameObject);

        return objects;
    }
}
