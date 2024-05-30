using System;
using System.Collections;
using BehaviorTree;
using UnityEngine;
using UnityEngine.VFX;

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
    [SerializeField] GameObject[] meshes;
    [SerializeField] EnemyType type;
    [SerializeField] GameObject enemyMainGameObject;
    [SerializeField] VisualEffect smokeVFX;
    [SerializeField] Sound[] spearHitSounds;
    [SerializeField] Sound[] swordHitSounds;
    [SerializeField] Sound[] hammerHitSounds;
    [SerializeField] HealthBar healthBar;

    int currentHealth;

    void OnEnable() {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage, WeaponDamageType? damageType) {

        HitFlash();

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

        if (damageType != null) {
            PlayHitSound(damageType.Value);
        }

        currentHealth -= dam;
        healthBar.SetProgress((float)currentHealth / maxHealth);
        if (currentHealth <= 0) Kill();
    }

    public void PlayHitSound(WeaponDamageType damageType) {
        switch (damageType) {
            case WeaponDamageType.SPEAR:
                SoundFXManager.Instance.PlayRandom(spearHitSounds);
                break;
            case WeaponDamageType.SWORD:
                SoundFXManager.Instance.PlayRandom(swordHitSounds);
                break;
            case WeaponDamageType.HAMMER:
                SoundFXManager.Instance.PlayRandom(hammerHitSounds);
                break;
        }
    }

    public void Kill() {
        GameManager.Instance.AddKillCount(type);
        GameObject soul = Instantiate(Resources.Load("Level/Prefabs/VFX/Soul"), transform.position + Vector3.up, Quaternion.identity) as GameObject;
        soul.GetComponent<SoulVFX>().soulType = type;
        enemyMainGameObject.GetComponent<AITree>().SetActive(false);

        StartCoroutine(Disolve());
    }

    public void KillWithoutSoul() {
        enemyMainGameObject.GetComponent<AITree>().SetActive(false);
        StartCoroutine(Disolve());
    }

    public void Heal(int heal) {
        int newHealth = currentHealth + heal;
        int overheal = newHealth - maxHealth;
        if (overheal > 0) Overheal(overheal);

        currentHealth = Math.Min(maxHealth, newHealth);
        healthBar.SetProgress((float)currentHealth / maxHealth);
    }

    void Overheal(int overheal) {
        Debug.Log("Overheal: " + overheal);
    }

    public void Shield() {
        hasShield = true;
        if (meshes == null) return;
        foreach (GameObject mesh in meshes) {
            mesh.GetComponent<SkinnedMeshRenderer>().materials[0].SetFloat("_Alpha", 1);
            mesh.GetComponent<SkinnedMeshRenderer>().materials[0].SetFloat("_ShieldStrength", 2);
        }
    }

    public void BreakShield() {
        hasShield = false;
        if (meshes == null) return;

        foreach (GameObject mesh in meshes) {
            mesh.GetComponent<SkinnedMeshRenderer>().materials[0].SetFloat("_Alpha", 0);
            mesh.GetComponent<SkinnedMeshRenderer>().materials[0].SetFloat("_ShieldStrength", 0);
        }
    }

    void HitFlash() {

        if (meshes == null) return;
        foreach (GameObject mesh in meshes) {
            mesh.GetComponent<SkinnedMeshRenderer>().materials[0].SetInt("_HitFlashBool", 1);
        }

        Invoke(nameof(ResetHitFlash), 0.05f);

    }

    void ResetHitFlash() {

        if (meshes == null) return;
        foreach (GameObject mesh in meshes) {
            mesh.GetComponent<SkinnedMeshRenderer>().materials[0].SetInt("_HitFlashBool", 0);
        }

    }

    private void OnDisable() {
        enemyMainGameObject.SetActive(false);
    }

    IEnumerator Disolve() {
        GetComponent<Collider>().enabled = false;
        smokeVFX.Play();

        float time = 2f;
        while (time >= 0) {
            float prog = 1 - time / 2f;
            foreach (GameObject mesh in meshes) {
                mesh.GetComponent<SkinnedMeshRenderer>().materials[0].SetFloat("_Dissolve_Amount", prog);
            }

            time -= Time.deltaTime;
            yield return null;
        }

        foreach (GameObject mesh in meshes) {
            mesh.GetComponent<SkinnedMeshRenderer>().materials[0].SetFloat("_Dissolve_Amount", 0f);
        }

        enemyMainGameObject.GetComponent<AITree>().SetActive(false);
        enemyMainGameObject.SetActive(false);
        GetComponent<Collider>().enabled = true;
        SpawnManager.Instance.EnqueueEnemy(enemyMainGameObject);
    }
}
