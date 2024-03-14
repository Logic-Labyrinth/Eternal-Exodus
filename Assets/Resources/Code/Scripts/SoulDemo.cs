using UnityEngine;

public class SoulDemo : MonoBehaviour {
    public GameObject soul;
    public float radius = 50f;
    public float interval = 0.5f;
    public bool spawn = true;

    float circleRad;

    void Start() {
        circleRad = 2 * Mathf.PI;
        if (spawn)
            InvokeRepeating(nameof(Spawn), 0, interval);
    }

    void Spawn() {
        float angle = Random.Range(0, circleRad);
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        Instantiate(soul, new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z), Quaternion.identity);
    }
}
