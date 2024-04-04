using System;
using TMPro;
using UnityEngine;

public enum WeaponDamageType {
    SPEAR,
    SWORD,
    HAMMER
}

public class HealthSystem : MonoBehaviour {
    [SerializeField] int maxHealth = 100;
    [SerializeField, Range(0, 100)] int weaknessFactor = 50;
    [SerializeField, Range(0, 100)] int resistanceFactor = 50;
    [SerializeField] WeaponDamageType weakness;
    [SerializeField] WeaponDamageType resistance;
    [SerializeField] bool hasShield = false;
    int currentHealth;

    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI shieldedText;

    private SpawnManager spawnManager;

    public int GetHealth() { return currentHealth; }
    public int GetMaxHealth() { return maxHealth; }
    public bool HasShield() { return hasShield; }

    void Start() {
        currentHealth = maxHealth;
        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
    }

    void Update() {
        if (healthText && shieldedText) {
            healthText.text = currentHealth.ToString();
            if (hasShield) {
                shieldedText.text = "Shielded";
                shieldedText.color = Color.blue;
            } else {
                shieldedText.text = "No Shield";
                shieldedText.color = Color.red;
            }
        }

    }

    public void TakeDamage(int damage, WeaponDamageType? damageType, Vector3 hitLocation) {
        Debug.Log("Damage: " + damage + ", Type: " + damageType);
        if (hasShield) {
            BreakShield();
            return;
        }

        int dam = damage;
        if (damageType == weakness) {
            dam += (int)Math.Floor(dam * weaknessFactor / 100.0f);
        } else if (damageType == resistance) {
            dam -= (int)Math.Floor(dam * resistanceFactor / 100.0f);
        }

        Debug.Log("Dam: " + dam);
        GameObject hitVFXPrefab = Resources.Load<GameObject>("Level/Prefabs/VFX/HitVFX");
        Instantiate(hitVFXPrefab, hitLocation, Quaternion.identity).GetComponent<HitVFX>().Play(dam);
        currentHealth -= dam;
        if (currentHealth <= 0) Kill();
    }

    public void Kill() {
        // Kill the entity
        Debug.Log("I died!");
        Instantiate(Resources.Load("Level/Prefabs/VFX/Soul"), transform.position + Vector3.up, Quaternion.identity);
        gameObject.SetActive(false);
        spawnManager.EnqueueEnemy(gameObject);
    }

    public void Heal(int heal) {
        int newHealth = currentHealth + heal;
        int overheal = newHealth - maxHealth;
        if (overheal > 0) Overheal(overheal);

        currentHealth = Math.Min(maxHealth, newHealth);
    }

    void Overheal(int overheal) {
        Debug.Log("Overheal: " + overheal);
    }

    public void Shield() {
        hasShield = true;
    }

    void BreakShield() {
        // Stuff for breaking the shield
        hasShield = false;
    }
}
