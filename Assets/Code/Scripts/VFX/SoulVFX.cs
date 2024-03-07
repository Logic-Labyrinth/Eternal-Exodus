using UnityEngine;

public class SoulVFX : MonoBehaviour {
    [SerializeField] float radius = 1f;
    // [SerializeField] Vector3 center;
    [SerializeField] float speed = 10f;

    float angle = 0;

    void Start() {

    }

    void Update() {
        angle = (angle + Time.deltaTime * speed) % 360;
        float x = radius * Mathf.Cos(Mathf.Deg2Rad * angle);
        float y = radius * Mathf.Sin(Mathf.Deg2Rad * angle);

        // transform.position = center + new Vector3(x, y, 0);
        transform.localPosition = new Vector3(x, y, 0);
    }
}
