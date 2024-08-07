using Sirenix.Utilities;
using TEE.Health;
using UnityEngine;
using UnityEngine.VFX;

namespace TEE.Enemy {
    public class BishopBuff : MonoBehaviour {
        [SerializeField] GameObject buffAura;
        [SerializeField] float      range = 10f;

        void Start() {
            buffAura.GetComponent<VisualEffect>().SetFloat("Size", range * 2);
        }

        void FixedUpdate() {
            var colliders = Physics.OverlapSphere(transform.position, range);

            colliders.ForEach(c => {
                if (c.gameObject.CompareTag("Pawn")) c.GetComponent<HealthSystem>().Shield();
            });
        }
    }
}