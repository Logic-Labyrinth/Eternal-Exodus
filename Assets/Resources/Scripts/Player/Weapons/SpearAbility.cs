using TEE.Environment;
using TEE.Health;
using TEE.Player.Movement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

namespace TEE.Player.Weapons {
    public class SpecialAbilityDamage : MonoBehaviour {
        [SerializeField] Weapon           spearData;
        [SerializeField] VisualEffect     visualEffect;
        [SerializeField] BasicFreezeFrame basicFreezeFrame;

        OLD_PlayerMovement pm;

        void Start() {
            pm = GameObject.Find("Player").GetComponent<OLD_PlayerMovement>();
        }

        void OnTriggerEnter(Collider other) {
            if (!pm.dashing) return;
            visualEffect.Play();
            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
                other.GetComponent<HealthSystem>()?.TakeDamage(spearData.baseDamage, WeaponDamageType.Spear);
                other.GetComponent<NavMeshAgent>().isStopped      = true;
                other.GetComponent<NavMeshAgent>().updatePosition = false;

                // add force away from spear position and up
                other.GetComponent<Rigidbody>().AddForce((other.transform.position - transform.position).normalized * 5 + Vector3.up * 15, ForceMode.Impulse);
                FrameHang.Instance.ExecFrameHang(basicFreezeFrame, 0.01f);
            }

            if (other.CompareTag("Breakable")) other.GetComponent<BreakableObject>().Break();
        }
    }
}