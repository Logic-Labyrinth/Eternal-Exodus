using UnityEngine;
using UnityEngine.UI;

public class PlayerShieldUIController : MonoBehaviour {
    [SerializeField] Image shieldImage;
    [SerializeField] Color fullShieldColor;
    [SerializeField] Sprite fullShieldSprite;
    [SerializeField] Color damagedShieldColor;
    [SerializeField] Sprite damagedShieldSprite;
    [SerializeField] float alphaSpeed;
    [SerializeField] int alphaRange;

    struct ShieldColorData {
        public Color color;
        public Sprite sprite;
    }

    void Awake() {
        shieldImage = GetComponent<Image>();
        // shieldImage.material.SetFloat("_alpha", )
    }

    void Update() {
        // if (!shieldImage.enabled) return;
        // if (alphaRange == 0) return;
        // float sin = Mathf.Sin(Time.time * alphaSpeed) * alphaRange;
        // Debug.Log(sin + " " + shieldImage.color.a);
        // shieldImage.color = new Color(shieldImage.color.r, shieldImage.color.g, shieldImage.color.b, sin + shieldImage.color.a);
    }

    public void Shield() {
        // 00D4FF, alpha: 25-45
        shieldImage.enabled = true;
        shieldImage.sprite = fullShieldSprite;
        shieldImage.color = fullShieldColor;
        shieldImage.material.SetTexture("_FullShield", fullShieldSprite.texture);
        shieldImage.material.SetColor("_Color", fullShieldColor);
    }

    public void DamageShield() {
        shieldImage.enabled = true;
        shieldImage.sprite = damagedShieldSprite;
        shieldImage.color = damagedShieldColor;
        shieldImage.material.SetTexture("_FullShield", damagedShieldSprite.texture);
        shieldImage.material.SetColor("_Color", damagedShieldColor);
    }

    public void BreakShield() {
        shieldImage.enabled = false;
    }
}
