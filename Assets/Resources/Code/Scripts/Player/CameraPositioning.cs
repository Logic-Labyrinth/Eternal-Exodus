using UnityEngine;

public class CameraPositioning : MonoBehaviour {
    public static CameraPositioning Instance { get; private set; }
    [SerializeField] Transform trans;

    Vector3 Shake = Vector3.zero;
    Quaternion CamRot = Quaternion.identity;

    Vector3 maxTranslationShake = Vector3.one;
    Vector3 maxRotationShake = Vector3.one * 15;
    float frequency = 25;
    float traumaExponent = 1;
    float recoverySpeed = 1;
    float trauma, seed;

    void Awake() {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
        seed = Random.value;
    } 

    void Update() {
        float shake = Mathf.Pow(trauma, traumaExponent);
        Shake = new Vector3(
            maxTranslationShake.x * (Mathf.PerlinNoise(seed, Time.unscaledTime * frequency) * 2 - 1),
            maxTranslationShake.y * (Mathf.PerlinNoise(seed + 1, Time.unscaledTime * frequency) * 2 - 1),
            maxTranslationShake.z * (Mathf.PerlinNoise(seed + 2, Time.unscaledTime * frequency) * 2 - 1)
        ) * shake;

        CamRot = Quaternion.Euler(new Vector3(
            maxRotationShake.x * (Mathf.PerlinNoise(seed + 3, Time.unscaledTime * frequency) * 2 - 1),
            maxRotationShake.y * (Mathf.PerlinNoise(seed + 4, Time.unscaledTime * frequency) * 2 - 1),
            maxRotationShake.z * (Mathf.PerlinNoise(seed + 5, Time.unscaledTime * frequency) * 2 - 1)
        ) * shake);

        transform.position = trans.position + Shake;

        trauma = Mathf.Clamp01(trauma - Time.unscaledDeltaTime * recoverySpeed);
    }

    void LateUpdate() {
        transform.rotation = transform.rotation * CamRot;
    }

    public void InduceStress(float stress) {
        trauma = Mathf.Clamp01(trauma + stress);
    }
}
