using UnityEngine;

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
        // other.transform.parent.GetComponent<HealthSystem>().TakeDamage(spearData.baseDamage, WeaponDamageType.SPEAR, transform.position);
    }
}
