using UnityEngine;
using UnityEngine.UI;

namespace TEE.UI {
    public class PlayerShieldUIController : MonoBehaviour {
        [SerializeField] Image  shieldImage;
        [SerializeField] Color  shieldColorTwo;
        [SerializeField] Sprite shieldSpriteTwo;
        [SerializeField] Color  shieldColorOne;
        [SerializeField] Sprite shieldSpriteOne;
        [SerializeField] Color  damagedShieldColor;
        [SerializeField] Sprite damagedShieldSprite;
        static readonly  int    ShaderPropertyFullShield = Shader.PropertyToID("_FullShield");
        static readonly  int    ShaderPropertyColor      = Shader.PropertyToID("_Color");

        struct ShieldColorData {
            public Color  color;
            public Sprite sprite;
        }

        void Awake() {
            shieldImage         = GetComponent<Image>();
            shieldImage.enabled = false;
        }

        public void ShieldTwo() {
            shieldImage.enabled = true;
            shieldImage.sprite  = shieldSpriteTwo;
            shieldImage.color   = shieldColorTwo;
            shieldImage.material.SetTexture(ShaderPropertyFullShield, shieldSpriteTwo.texture);
            shieldImage.material.SetColor(ShaderPropertyColor, shieldColorTwo);
        }

        public void ShieldOne() {
            shieldImage.enabled = true;
            shieldImage.sprite  = shieldSpriteOne;
            shieldImage.color   = shieldColorOne;
            shieldImage.material.SetTexture(ShaderPropertyFullShield, shieldSpriteOne.texture);
            shieldImage.material.SetColor(ShaderPropertyColor, shieldColorOne);
        }

        public void DamageShield() {
            shieldImage.enabled = true;
            shieldImage.sprite  = damagedShieldSprite;
            shieldImage.color   = damagedShieldColor;
            shieldImage.material.SetTexture(ShaderPropertyFullShield, damagedShieldSprite.texture);
            shieldImage.material.SetColor(ShaderPropertyColor, damagedShieldColor);
        }

        public void BreakShield() {
            shieldImage.enabled = false;
        }
    }
}