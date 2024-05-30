using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class CameraPositioning : MonoBehaviour {
    public static CameraPositioning Instance { get; private set; }
    [SerializeField] Transform trans;
    [SerializeField] float radius = 1f;

    Vector3 Shake = Vector3.zero;
    Vector3 CamRot = Vector3.zero;

    void Awake() {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    Vector2 PolarToCartesian(float r, float a) {
        float x = r * Mathf.Cos(a);
        float y = r * Mathf.Sin(a);

        return new Vector2(x, y);
    }

    void Update() {
        transform.position = trans.position + Shake;
    }

    void LateUpdate() {
        transform.rotation = Quaternion.Euler(CamRot + transform.rotation.eulerAngles);
    }

    public void ShakeCamera(AnimationCurve curve, float duration, float multiplier = 1) {
        StartCoroutine(CamShake(curve, duration, multiplier));
    }

    IEnumerator CamShake(AnimationCurve curve, float duration, float multiplier = 1) {
        float elapsed = 0f;
        float angle = 0f;

        while (elapsed < duration) {
            float magnitude = curve.Evaluate(elapsed / duration) * multiplier;

            angle += Time.deltaTime / duration * 360;

            Vector2 PerlinCoord_X = PolarToCartesian(radius, angle);
            Vector2 PerlinCoord_Y = PolarToCartesian(radius, angle);

            float Noise_X = Mathf.PerlinNoise(PerlinCoord_X.x, PerlinCoord_X.y) * 2 - 1;
            float Noise_Y = Mathf.PerlinNoise(PerlinCoord_Y.x, PerlinCoord_Y.y) * 2 - 1;

            float x1 = Noise_X * ((1 - elapsed / duration) * magnitude);
            float y1 = Noise_X * ((1 - elapsed / duration) * magnitude);

            float x2 = Noise_Y * ((1 - elapsed / duration) * magnitude);
            float y2 = Noise_Y * ((1 - elapsed / duration) * magnitude);

            Shake.x = x1;
            Shake.y = y1;

            CamRot.x = x2;
            CamRot.y = y2;

            elapsed += Time.deltaTime;

            yield return null;
        }

        Shake = Vector3.zero;
        CamRot = Vector3.zero;
    }
}
