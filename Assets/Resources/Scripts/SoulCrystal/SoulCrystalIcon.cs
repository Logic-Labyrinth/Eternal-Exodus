using UnityEngine;
using UnityEngine.UI;

namespace TEE.SoulCrystal {
    public class SoulCrystalIcon : MonoBehaviour {
        Material            material;
        static readonly int ShaderPropertyBlink = Shader.PropertyToID("_Blink");

        void Awake() {
            material = GetComponent<Image>().material;
            material.SetInt(ShaderPropertyBlink, 0);
        }

        public void SetBlink(bool blink) {
            material.SetInt(ShaderPropertyBlink, blink ? 1 : 0);
        }
    }
}