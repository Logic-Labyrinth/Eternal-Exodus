using UnityEngine;

public class SoulCrystalIconUI : MonoBehaviour {
    [SerializeField] GameObject trackedObject;
    [SerializeField] Vector3 offset;
    new Camera camera;
    Vector2 imageSize;
    float halfHeight, halfWidth;

    void Start() {
        camera = Camera.main;
        Debug.Log(camera);
        imageSize = GetComponent<RectTransform>().sizeDelta;
        halfHeight = imageSize.y / 2;
        halfWidth = imageSize.x / 2;
    }

    void Update() {
        Vector3 pos = camera.WorldToScreenPoint(trackedObject.transform.position + offset);

        // if (pos.y + halfHeight > Screen.height) pos.y = Screen.height - halfHeight;
        // if (pos.y - halfHeight < 0) pos.y = halfHeight;
        // if (pos.x + halfWidth > Screen.width) pos.x = Screen.width - halfWidth;
        // if (pos.x - halfWidth < 0) pos.x = halfWidth;

        if (transform.position != pos) transform.position = pos;
    }
}