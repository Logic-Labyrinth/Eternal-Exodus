using LexUtils.Events;
using LexUtils.Singleton;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TEE.Player.Camera {
    public class CameraPositioning : Singleton<CameraPositioning> {
        [SerializeField] Transform targetTransform;

        Vector3    shake  = Vector3.zero;
        Quaternion camRot = Quaternion.identity;

        readonly Vector3 maxTranslationShake = Vector3.one;
        readonly Vector3 maxRotationShake    = Vector3.one * 15;
        const    float   Frequency           = 25;
        const    float   TraumaExponent      = 1;
        const    float   RecoverySpeed       = 1;
        float            seed;
        float            trauma;

        void Start() {
            seed = Random.value;
            EventForge.Float.Get("Player.Trauma").AddListener(stress => trauma = Mathf.Clamp01(trauma + stress));
        }

        void Update() {
            float shakeAmount = Mathf.Pow(trauma, TraumaExponent);
            shake = new Vector3(
                maxTranslationShake.x * (Mathf.PerlinNoise(seed,     Time.unscaledTime * Frequency) * 2 - 1),
                maxTranslationShake.y * (Mathf.PerlinNoise(seed + 1, Time.unscaledTime * Frequency) * 2 - 1),
                maxTranslationShake.z * (Mathf.PerlinNoise(seed + 2, Time.unscaledTime * Frequency) * 2 - 1)
            ) * shakeAmount;

            camRot = Quaternion.Euler(new Vector3(
                maxRotationShake.x * (Mathf.PerlinNoise(seed + 3, Time.unscaledTime * Frequency) * 2 - 1),
                maxRotationShake.y * (Mathf.PerlinNoise(seed + 4, Time.unscaledTime * Frequency) * 2 - 1),
                maxRotationShake.z * (Mathf.PerlinNoise(seed + 5, Time.unscaledTime * Frequency) * 2 - 1)
            ) * shakeAmount);

            transform.position = targetTransform.position + shake;

            trauma = Mathf.Clamp01(trauma - Time.unscaledDeltaTime * RecoverySpeed);
        }

        void LateUpdate() {
            transform.rotation *= camRot;
        }
    }
}