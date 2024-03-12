using UnityEngine;

public class SoulVFX : MonoBehaviour {
    [SerializeField] float radius = 1f;
    [SerializeField] float speed = 10f;
    [SerializeField] Transform vfxTransform;

    Vector3 target;
    float angle = 0;

    void Start() {
        target = GameObject.Find("Crystal").transform.position;
        transform.LookAt(target);
    }

    void Update() {
        transform.position = Vector3.MoveTowards(transform.position, target, 0.1f);
        if (Vector3.Distance(transform.position, target) < 0.1f) {
            Destroy(gameObject);
        }


        // angle = (angle + Time.deltaTime * speed) % 360;
        // float x = radius * Mathf.Cos(Mathf.Deg2Rad * angle);
        // float y = radius * Mathf.Sin(Mathf.Deg2Rad * angle);
        // vfxTransform.position = transform.position + new Vector3(x, y, 0);

        // transform.RotateAroundLocal(Vector3.forward, Mathf.Deg2Rad * angle);
    }
}
