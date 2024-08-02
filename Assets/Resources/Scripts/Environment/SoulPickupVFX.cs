using UnityEngine;

namespace TEE.Environment {
    public class SoulPickupVFX : MonoBehaviour {
        public           float soulValue = 1f;
        [SerializeField] float speed     = 0.1f;
        Vector3                target, origin;
        float                  timer;

        void Start() {
            target = GameObject.Find("Soul Collector").transform.position;
            origin = transform.position;
        }

        void FixedUpdate() {
            transform.position =  Vector3.Slerp(origin, target, timer * speed);
            timer              += Time.fixedDeltaTime;
        }
    }
}