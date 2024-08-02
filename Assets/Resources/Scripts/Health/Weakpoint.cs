using UnityEngine;

namespace TEE.Health {
    public class Weakpoint : MonoBehaviour {
        [SerializeField] HealthSystem healthSystem;
        [SerializeField] float        damageMultiplier = 1f;

        public void TakeDamage(int damage, WeaponDamageType? damageType) {
            if (healthSystem == null) return;

            healthSystem.TakeDamage(Mathf.FloorToInt(damage * damageMultiplier), damageType);

            gameObject.SetActive(false);
        }
    }
}