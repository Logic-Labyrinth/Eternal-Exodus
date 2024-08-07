using LexUtils.Events;
using LexUtils.Singleton;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TEE.Player.Camera {
    public class CameraShake : Singleton<CameraShake> {
        readonly Vector3 maxTranslationShake = Vector3.one;
        readonly Vector3 maxRotationShake    = Vector3.one * 15;
        const    float   Frequency           = 25;
        const    float   TraumaExponent      = 1;
        const    float   RecoverySpeed       = 1;
        float            seed;
        float            trauma;
        Vector3          position;

        void Start() {
            seed     = Random.value;
            position = transform.localPosition;
            EventForge.Float.Get("Player.Trauma").AddListener(stress => trauma = Mathf.Clamp01(trauma + stress));
        }

        void Update() {
            float shakeAmount = Mathf.Pow(trauma, TraumaExponent);
            Vector3 shake = new Vector3(
                maxTranslationShake.x * (Mathf.PerlinNoise(seed,     Time.unscaledTime * Frequency) * 2 - 1),
                maxTranslationShake.y * (Mathf.PerlinNoise(seed + 1, Time.unscaledTime * Frequency) * 2 - 1),
                maxTranslationShake.z * (Mathf.PerlinNoise(seed + 2, Time.unscaledTime * Frequency) * 2 - 1)
            ) * shakeAmount;

            Quaternion camRot = Quaternion.Euler(new Vector3(
                maxRotationShake.x * (Mathf.PerlinNoise(seed + 3, Time.unscaledTime * Frequency) * 2 - 1),
                maxRotationShake.y * (Mathf.PerlinNoise(seed + 4, Time.unscaledTime * Frequency) * 2 - 1),
                maxRotationShake.z * (Mathf.PerlinNoise(seed + 5, Time.unscaledTime * Frequency) * 2 - 1)
            ) * shakeAmount);

            transform.localPosition =  position + shake;
            transform.rotation      *= camRot;

            trauma = Mathf.Clamp01(trauma - Time.unscaledDeltaTime * RecoverySpeed);
        }
    }
}