using UnityEngine;

namespace TEE.VFX {
    public class SoulVFX : MonoBehaviour {
        [SerializeField] float     speed    = 0.1f;
        public           EnemyType soulType = EnemyType.Pawn;
        Vector3                    target, origin;
        float                      timer = 0;

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