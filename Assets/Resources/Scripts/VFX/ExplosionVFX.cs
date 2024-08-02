using UnityEngine;
using UnityEngine.VFX;

namespace TEE.VFX {
    [RequireComponent(typeof(VisualEffect))]
    public class ExplosionVFX : MonoBehaviour {
        VisualEffect explosionEffect;

        void Awake() {
            explosionEffect = GetComponent<VisualEffect>();
            explosionEffect.Stop();
        }

        public void Play() {
            explosionEffect.Play();
        }

        public float GetDuration() {
            return explosionEffect.GetFloat("Lifetime");
        }
    }
}