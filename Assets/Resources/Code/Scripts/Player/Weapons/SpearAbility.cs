using UnityEngine;

public class SpecialAbilityDamage : MonoBehaviour {
    [SerializeField] Weapon spearData;

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer != LayerMask.NameToLayer("Enemy")) return;
        other.GetComponent<HealthSystem>().TakeDamage(spearData.baseDamage, WeaponDamageType.SPEAR, transform.position);
        // other.transform.parent.GetComponent<HealthSystem>().TakeDamage(spearData.baseDamage, WeaponDamageType.SPEAR, transform.position);
    }
}
