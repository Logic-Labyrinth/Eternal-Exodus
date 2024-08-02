using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TEE.Player.Weapons {
    public static class CustomCapsuleCollider {
        // It's just a chonky RaycastAll really...
        public static List<GameObject> GetAllObjects(Transform t, Vector3 direction, float radius, float distance) {
            var hits = Physics.SphereCastAll(t.position, radius, direction, distance);
            return hits.Select(hit => hit.transform.gameObject).ToList();
        }
    }
}