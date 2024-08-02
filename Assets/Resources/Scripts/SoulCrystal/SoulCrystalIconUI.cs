using UnityEngine;

namespace TEE.SoulCrystal {
    public class SoulCrystalIconUI : MonoBehaviour {
        [SerializeField]              GameObject trackedObject;
        [SerializeField]              Vector3    offset;
        [SerializeField, Range(0, 1)] float      radiusPercentage = 1;

        new Camera camera;

        void Start() {
            camera = Camera.main;
        }

        void LateUpdate() {
            // Position the UI icon on the tracked object
            Vector2 pos = camera.WorldToViewportPoint(trackedObject.transform.position + offset);

            // Check if the object is in front of the camera
            if (Vector3.Dot(trackedObject.transform.position - transform.position, transform.forward) < 0)
                pos = -pos;

            Vector2 posClamp = 0.5f * radiusPercentage * (Vector2.one * 0.5f - pos).normalized;

            if ((Vector2.one * 0.5f - pos).magnitude > posClamp.magnitude) pos = -posClamp + Vector2.one * 0.5f;
            pos.x -= 0.5f;
            pos.y -= 0.5f;

            pos = camera.ViewportToScreenPoint(pos);

            GetComponent<RectTransform>().anchoredPosition = pos;
        }
    }
}