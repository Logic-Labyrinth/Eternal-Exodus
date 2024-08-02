using UnityEngine;
using UnityEngine.VFX;

namespace TEE.VFX {
    public class LandingVFX : MonoBehaviour {
        [SerializeField]               VisualEffect vfx;
        [SerializeField, Range(0, 10)] int          lifespan  = 5;
        [SerializeField, Range(0, 50)] int          threshold = 5;

        public void Play(float intensity) {
            if (intensity < threshold) Destroy(gameObject);

            gameObject.transform.localScale = intensity / threshold * Vector3.one;
            Destroy(gameObject, lifespan);
        }
    }
}