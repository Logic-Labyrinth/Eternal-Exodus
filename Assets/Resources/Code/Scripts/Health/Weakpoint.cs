using UnityEngine;

public class Weakpoint : MonoBehaviour {
    [SerializeField] HealthSystem healthSystem;
    [SerializeField] float damageMultiplier = 1f;

    public void TakeDamage(int damage, WeaponDamageType? damageType, Vector3 hitLocation) {
        if (healthSystem == null) return;

        healthSystem.TakeDamage(Mathf.FloorToInt(damage * damageMultiplier), damageType, hitLocation);

        gameObject.SetActive(false);
    }
}
