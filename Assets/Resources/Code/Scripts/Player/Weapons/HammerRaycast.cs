using UnityEngine;

public class HammerRaycast : MonoBehaviour {
    [SerializeField] float distance = 2f;
    [SerializeField] float verticalPadding = 0.01f;
    [SerializeField] LayerMask layer;

    public Vector3 Raycast() {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, distance, layer)) {
            GameObject go = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere), hit.point, Quaternion.identity);
            go.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            return hit.point + verticalPadding * transform.up;
        }
        return Vector3.zero;
    }
}
