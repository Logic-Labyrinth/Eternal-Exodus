using UnityEngine;
using UnityEngine.VFX;

namespace TEE.VFX {
    [RequireComponent(typeof(VisualEffect))]
    public class DustTrailVFX : MonoBehaviour {
        [SerializeField, Range(0, 50)] int threshold = 5;

        VisualEffect vfx;
        float        speed;
        bool         canPlay;

        void Awake() {
            vfx = GetComponent<VisualEffect>();
        }

        void FixedUpdate() {
            if (!canPlay || speed < threshold) {
                vfx.Stop();
                return;
            }

            vfx.Play();
        }

        public void SetSpeed(float s) {
            speed = s;
        }

        public void SetCanPlay(bool b) {
            canPlay = b;
        }
    }
}