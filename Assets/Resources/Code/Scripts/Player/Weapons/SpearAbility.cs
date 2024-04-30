using UnityEngine;
using UnityEngine.AI;

public class SpecialAbilityDamage : MonoBehaviour {
    [SerializeField] Weapon spearData;
    PlayerMovement pm;

    void Start() {
        pm = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer != LayerMask.NameToLayer("Enemy")) return;
        if (!pm.dashing) return;
        other.GetComponent<HealthSystem>().TakeDamage(spearData.baseDamage, WeaponDamageType.SPEAR, transform.position);
        other.GetComponent<NavMeshAgent>().isStopped = true;
        other.GetComponent<NavMeshAgent>().updatePosition = false;

        // add force away from spear position and up
        other.GetComponent<Rigidbody>().AddForce((other.transform.position - transform.position).normalized * 5 + Vector3.up * 15, ForceMode.Impulse);
        // other.transform.parent.GetComponent<HealthSystem>().TakeDamage(spearData.baseDamage, WeaponDamageType.SPEAR, transform.position);
    }
}
