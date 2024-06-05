using UnityEngine;

public class SoulCrystalIconUI : MonoBehaviour {
    [SerializeField] GameObject trackedObject;
    [SerializeField] Vector3 offset;
    [SerializeField, Range(0, 1)] float radiusPercentage = 1;

    new Camera camera;
    void Start() {
        camera = Camera.main;
    }

    void Update() {
        Vector2 pos = camera.WorldToViewportPoint(trackedObject.transform.position + offset);

        if(Vector3.Dot(trackedObject.transform.position - transform.position, transform.forward) < 0) pos = -pos;

        Vector2 posClamp = (Vector2.one * 0.5f - pos).normalized * 0.5f * radiusPercentage;

        Debug.Log("Pos Clamp: " + posClamp);
        if((Vector2.one * 0.5f - pos).magnitude > posClamp.magnitude) pos = -posClamp + Vector2.one * 0.5f;
        pos.x -= 0.5f;
        pos.y -= 0.5f;
        
        pos = camera.ViewportToScreenPoint(pos * 0.5f);
        GetComponent<RectTransform>().anchoredPosition = pos;
    }
}
