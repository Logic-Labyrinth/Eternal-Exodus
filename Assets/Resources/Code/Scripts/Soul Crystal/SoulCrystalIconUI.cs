using UnityEngine;
using UnityEngine.UI;

public class SoulCrystalIconUI : MonoBehaviour {
    [SerializeField] GameObject trackedObject;
    [SerializeField] Vector3 offset;
    [SerializeField] RectTransform canvasTransform;
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
        minX = image.GetPixelAdjustedRect().width / 2;
        minY = image.GetPixelAdjustedRect().height / 2;
        maxX = Screen.width / 2 - minX;
        maxY = Screen.height / 2 - minY;
    }

    void Update() {
        Vector2 pos = camera.WorldToScreenPoint(trackedObject.transform.position + offset);

        pos.x *= canvasTransform.rect.width / camera.pixelWidth;
        pos.y *= canvasTransform.rect.height / camera.pixelHeight;

        if(Vector3.Dot(trackedObject.transform.position - transform.position, transform.forward) < 0){
            if(pos.x < Screen.width / 4) pos.x = maxX;
            else pos.x = minX;
        }

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        pos.x -= halfScreenWidth;
        pos.y -= halfScreenHeight;

        GetComponent<RectTransform>().anchoredPosition = pos;
    }
}