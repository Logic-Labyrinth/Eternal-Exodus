using UnityEngine;

public class SoulCrystalIconUI : MonoBehaviour {
    [SerializeField] GameObject trackedObject;
    [SerializeField] Vector3 offset;
    [SerializeField] RectTransform canvasTransform;
    new Camera camera;
    Vector2 imageSize;
    float halfHeight, halfWidth;
    float halfScreenHeight, halfScreenWidth;

    void Start() {
        camera = Camera.main;
        Debug.Log(camera);
        imageSize = GetComponent<RectTransform>().sizeDelta;
        canvasTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        halfHeight = imageSize.y / 2;
        halfWidth = imageSize.x / 2;
        halfScreenHeight = Screen.height / 4;
        halfScreenWidth = Screen.width / 4;
        Debug.Log(halfScreenHeight);
        Debug.Log(halfScreenWidth);
    }

    void Update() {
        Vector2 pos = camera.WorldToScreenPoint(trackedObject.transform.position + offset);

        pos.x *= canvasTransform.rect.width / camera.pixelWidth;
        pos.y *= canvasTransform.rect.height / camera.pixelHeight;

        pos.x -= halfScreenWidth;
        pos.y -= halfScreenHeight;

        if (pos.y > halfScreenHeight - halfHeight) pos.y = halfScreenHeight - halfHeight;
        if (pos.y < -halfScreenHeight + halfHeight) pos.y = -halfScreenHeight + halfHeight;
        if (pos.x > halfScreenWidth - halfWidth) pos.x = halfScreenWidth - halfWidth;
        if (pos.x < -halfScreenWidth + halfWidth) pos.x = -halfScreenWidth + halfWidth;

        GetComponent<RectTransform>().anchoredPosition = pos;
        // GetComponent<RectTransform>().rotation = Quaternion.identity;
        transform.rotation = Quaternion.identity;
        // transform.LookAt(camera.transform);
    }
}