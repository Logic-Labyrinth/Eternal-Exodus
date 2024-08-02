using UnityEngine;
using UnityEngine.VFX;

namespace TEE.VFX {
    [RequireComponent(typeof(VisualEffect))]
    public class SpeedLinesVFX : MonoBehaviour {
        [SerializeField, Range(0, 50)] int threshold = 5;
        VisualEffect                       vfx;
        float                              speed;

        void Awake() {
            vfx = GetComponent<VisualEffect>();
        }

        void Update() {
            if (speed > threshold) {
                vfx.Play();
                return;
            }

            vfx.Stop();
        }

        public void SetSpeed(float s) {
            speed = s;
        }
    }
}