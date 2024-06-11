using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class SpecialAbilityDamage : MonoBehaviour {
    [SerializeField] Weapon spearData;
    [SerializeField] VisualEffect visualEffect;
    [SerializeField] BasicFreezeFrame basicFreezeFrame;

    PlayerMovement pm;

    void Start() {
        pm = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

    void OnTriggerEnter(Collider other) {
        if (!pm.dashing) return;
        visualEffect.Play();
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
            other.GetComponent<HealthSystem>()?.TakeDamage(spearData.baseDamage, WeaponDamageType.SPEAR);
            other.GetComponent<NavMeshAgent>().isStopped = true;
            other.GetComponent<NavMeshAgent>().updatePosition = false;

            // add force away from spear position and up
            other.GetComponent<Rigidbody>().AddForce((other.transform.position - transform.position).normalized * 5 + Vector3.up * 15, ForceMode.Impulse);
            FrameHang.Instance.ExecFrameHang(basicFreezeFrame, 0.01f);
        }
        if (other.CompareTag("Breakable")) other.GetComponent<BreakableObject>().Break();
    }
}
