using LexUtils.Singleton;
using UnityEngine;
using UnityEngine.UI;

namespace TEE.Health {
    public class HealthbarController : Singleton<HealthbarController> {
        [SerializeField] RectTransform healthbar;
        [SerializeField] float         minWidth;
        [SerializeField] Color         healthColor;
        [SerializeField] Color         weakShieldColor;
        [SerializeField] Color         strongShieldColor;

        float         originalWidth;
        RectTransform rectTransform;
        Image         image;

        // Shake settings
        Vector3          shake               = Vector3.zero;
        readonly Vector3 maxTranslationShake = Vector3.one;
        Vector3          originalPosition;
        const float      Frequency     = 25;
        const float      RecoverySpeed = 1;
        float            trauma, seed;

        protected override void Awake() {
            base.Awake();
            originalWidth    = healthbar.rect.width;
            image            = healthbar.GetComponent<Image>();
            rectTransform    = GetComponent<RectTransform>();
            originalPosition = rectTransform.anchoredPosition;
            seed             = Random.value;
        }

        void Update() {
            float shakeIntensity = trauma * 100;
            shake = new Vector3(
                maxTranslationShake.x * (Mathf.PerlinNoise(seed,     Time.unscaledTime * Frequency) * 2 - 1),
                maxTranslationShake.y * (Mathf.PerlinNoise(seed + 1, Time.unscaledTime * Frequency) * 2 - 1),
                0
            ) * shakeIntensity;

            rectTransform.anchoredPosition = originalPosition + shake;
            trauma = Mathf.Clamp01(trauma - Time.unscaledDeltaTime * RecoverySpeed);
        }

        public void InduceStress(float stress) {
            trauma = Mathf.Clamp01(trauma + stress);
        }

        public void SetProgress(float progress) {
            progress = Mathf.Clamp01(progress);
            float healthWidth = (originalWidth - minWidth) * progress + minWidth;
            healthbar.sizeDelta = new Vector2(healthWidth, healthbar.sizeDelta.y);
        }

        public void SetColor(byte status) {
            image.color = status switch {
                0 => healthColor,
                1 => weakShieldColor,
                2 => strongShieldColor,
                _ => healthColor
            };
        }
    }
}