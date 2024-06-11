using UnityEngine;
using UnityEngine.UI;

public class HealthbarController : MonoBehaviour {
    [SerializeField] RectTransform healthbar;
    [SerializeField] float minWidth;
    [SerializeField] Color healthColor;
    [SerializeField] Color weakShieldColor;
    [SerializeField] Color strongShieldColor;

    float originalWidth;
    RectTransform rectTransform;
    Image image;

    // Shake settings
    Vector3 Shake = Vector3.zero;
    Vector3 maxTranslationShake = Vector3.one;
    Vector3 originalPosition;
    float frequency = 25;
    float recoverySpeed = 1;
    float trauma, seed;

    public static HealthbarController Instance { get; private set; }

    void Awake() {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
        originalWidth = healthbar.rect.width;
        image = healthbar.GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        seed = Random.value;
    }

    void Update() {
        float shake = trauma * 100;
        Shake = new Vector3(
            maxTranslationShake.x * (Mathf.PerlinNoise(seed, Time.unscaledTime * frequency) * 2 - 1),
            maxTranslationShake.y * (Mathf.PerlinNoise(seed + 1, Time.unscaledTime * frequency) * 2 - 1),
            0
        ) * shake;

        rectTransform.anchoredPosition = originalPosition + Shake;

        trauma = Mathf.Clamp01(trauma - Time.unscaledDeltaTime * recoverySpeed);
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
