using UnityEngine;

public class SoulDemo2 : MonoBehaviour {
    public float intensity = 50f;
    public float height = 2f;
    float originalY;
    float timer = 0;

    void Update() {
        originalY = transform.parent.position.y;
        float y = Mathf.Sin( timer * intensity) * height;

        transform.position = new Vector3( transform.position.x, originalY + y, transform.position.z );
        timer += Time.deltaTime;
    }
}
