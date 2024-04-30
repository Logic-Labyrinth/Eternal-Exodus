using System;
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
    [SerializeField] GameObject mesh;
    [SerializeField] EnemyType type;
    [SerializeField] GameObject enemyMainGameObject;
    int currentHealth;

    private SpawnManager spawnManager;

    public int GetHealth() { return currentHealth; }
    public int GetMaxHealth() { return maxHealth; }
    public bool HasShield() { return hasShield; }

    void Start() {
        currentHealth = maxHealth;
        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
    }

    void OnEnable() {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage, WeaponDamageType? damageType, Vector3 hitLocation) {
        // Debug.Log("Damage: " + damage + ", Type: " + damageType);
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

        // Debug.Log("Dam: " + dam);
        GameObject hitVFXPrefab = Resources.Load<GameObject>("Level/Prefabs/VFX/HitVFX");
        Instantiate(hitVFXPrefab, hitLocation, Quaternion.identity).GetComponent<HitVFX>().Play(dam);
        currentHealth -= dam;
        if (currentHealth <= 0) Kill();
    }

    public void Kill() {
        // Kill the entity
        // Debug.Log("I died!");
        // EnemyType type = GetComponent<EnemyAI>().enemyType;
        GameManager.Instance.AddKillCount(type);
        GameObject soul = (GameObject)Instantiate(Resources.Load("Level/Prefabs/VFX/Soul"), transform.position + Vector3.up, Quaternion.identity);
        soul.GetComponent<SoulVFX>().soulType = type;
        enemyMainGameObject.SetActive(false);
        spawnManager.EnqueueEnemy(enemyMainGameObject);
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
        if (mesh == null) return;

        mesh.GetComponent<MeshRenderer>().materials[1].SetFloat("_ShieldStrength", 2);
    }

    public void BreakShield() {
        // Stuff for breaking the shield
        hasShield = false;
        if (mesh == null) return;
        mesh.GetComponent<MeshRenderer>().materials[1].SetFloat("_ShieldStrength", 0);
    }
}
