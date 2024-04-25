using UnityEngine;
using UnityEngine.AI;

public class SwordAbility : MonoBehaviour {
    [SerializeField] Weapon spearData;

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer != LayerMask.NameToLayer("Enemy")) return;
        // other.transform.parent.GetComponent<HealthSystem>().TakeDamage(spearData.baseDamage, WeaponDamageType.SWORD, transform.position);
        other.GetComponent<NavMeshAgent>().isStopped = true;
        other.GetComponent<EnemyAI>().enabled = false;
        // other.transform.parent.GetComponent<NavMeshAgent>().isStopped = true;
        // other.transform.parent.GetComponent<EnemyAI>().enabled = false;
        other.GetComponent<Rigidbody>().velocity = Vector3.zero;
        other.GetComponent<Rigidbody>().AddForce(Vector3.up * 50, ForceMode.Impulse);
    }
}
