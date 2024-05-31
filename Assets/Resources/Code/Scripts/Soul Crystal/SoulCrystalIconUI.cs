using UnityEngine;
using UnityEngine.UI;

public class SoulCrystalIconUI : MonoBehaviour {
    [SerializeField] GameObject trackedObject;
    [SerializeField] Vector3 offset;
    [SerializeField] RectTransform canvasTransform;
    [SerializeField] Vector2 edgeOffsetXY;

    new Camera camera;
    float halfScreenHeight, halfScreenWidth;
    float minX, maxX, minY, maxY;
    Image image;

    void Start() {
        camera = Camera.main;
        canvasTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        halfScreenHeight = Screen.height / 4;
        halfScreenWidth = Screen.width / 4;

        image = GetComponent<Image>();
        minX = image.GetPixelAdjustedRect().width / 2 + edgeOffsetXY.x;
        minY = image.GetPixelAdjustedRect().height / 2 + edgeOffsetXY.y;
        maxX = Screen.width / 2 - minX;
        maxY = Screen.height / 2 - minY;
    }

    void Update() {
        Vector2 pos = camera.WorldToScreenPoint(trackedObject.transform.position + offset);

        pos.x *= canvasTransform.rect.width / camera.pixelWidth;
        pos.y *= canvasTransform.rect.height / camera.pixelHeight;

        if (Vector3.Dot(trackedObject.transform.position - transform.position, transform.forward) < 0) {
            if (pos.x < Screen.width / 4) pos.x = maxX;
            else pos.x = minX;
        }

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        pos.x -= halfScreenWidth;
        pos.y -= halfScreenHeight;

        // pos = ClampToOval(pos, halfScreenWidth, halfScreenHeight);

        GetComponent<RectTransform>().anchoredPosition = pos;
    }

    Vector2 ClampToOval(Vector2 v, float width, float height) {
        float angleFromCenter = Vector2.SignedAngle(Vector2.up, v);
        float radius = width * height / Mathf.Sqrt(Mathf.Pow(height * Mathf.Cos(angleFromCenter), 2) + Mathf.Pow(width * Mathf.Sin(angleFromCenter), 2));

        float x = radius * Mathf.Cos(angleFromCenter);
        float y = radius * Mathf.Sin(angleFromCenter);

        Vector2 vector = new(x, y);

        if (Vector2.Distance(v, Vector2.zero) < radius) return v;

        return vector;
    }
}
