using UnityEngine;
using UnityEngine.UI;

public class PlayerShieldUIController : MonoBehaviour {
    [SerializeField] Image shieldImage;
    [SerializeField] Color shieldColorTwo;
    [SerializeField] Sprite shieldSpriteTwo;
    [SerializeField] Color shieldColorOne;
    [SerializeField] Sprite shieldSpriteOne;
    [SerializeField] Color damagedShieldColor;
    [SerializeField] Sprite damagedShieldSprite;

    struct ShieldColorData {
        public Color color;
        public Sprite sprite;
    }

    void Awake() {
        shieldImage = GetComponent<Image>();
    }

    public void ShieldTwo() {
        shieldImage.enabled = true;
        shieldImage.sprite = shieldSpriteTwo;
        shieldImage.color = shieldColorTwo;
        shieldImage.material.SetTexture("_FullShield", shieldSpriteTwo.texture);
        shieldImage.material.SetColor("_Color", shieldColorTwo);
    }

    public void ShieldOne() {
        shieldImage.enabled = true;
        shieldImage.sprite = shieldSpriteOne;
        shieldImage.color = shieldColorOne;
        shieldImage.material.SetTexture("_FullShield", shieldSpriteOne.texture);
        shieldImage.material.SetColor("_Color", shieldColorOne);
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
