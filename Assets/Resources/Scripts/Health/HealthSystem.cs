using System;
using System.Collections;
using LexUtils.Extensions;
using TEE.AI;
using TEE.Audio;
using TEE.Enemy;
using TEE.VFX;
using UnityEngine;
using UnityEngine.VFX;

namespace TEE.Health {
    public enum WeaponDamageType {
        Spear,
        Sword,
        Hammer
    }

    public class HealthSystem : MonoBehaviour {
        [SerializeField]                int              maxHealth        = 100;
        [SerializeField, Range(0, 100)] int              weaknessFactor   = 50;
        [SerializeField, Range(0, 100)] int              resistanceFactor = 50;
        [SerializeField]                WeaponDamageType weakness;
        [SerializeField]                WeaponDamageType resistance;
        [SerializeField]                bool             hasShield;
        [SerializeField]                GameObject[]     meshes;
        [SerializeField]                EnemyType        type;
        [SerializeField]                GameObject       enemyMainGameObject;
        [SerializeField]                VisualEffect     smokeVFX;
        [SerializeField]                VisualEffect     lightningVFX;
        [SerializeField]                Sound[]          spearHitSounds;
        [SerializeField]                Sound[]          swordHitSounds;
        [SerializeField]                Sound[]          hammerHitSounds;
        [SerializeField]                HealthBar        healthBar;

        int                   currentHealth;
        SkinnedMeshRenderer[] skinnedMeshes;
        Collider              enemyCollider;
        AITree                aiTree;
        static readonly int   ShaderPropertyShieldAmount   = Shader.PropertyToID("_ShieldAmount");
        static readonly int   ShaderPropertyHitFlashBool   = Shader.PropertyToID("_HitFlashBool");
        static readonly int   ShaderPropertyDissolveAmount = Shader.PropertyToID("_Dissolve_Amount");

        void Start() {
            aiTree        = enemyMainGameObject.GetComponent<AITree>();
            skinnedMeshes = enemyMainGameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            enemyCollider = enemyMainGameObject.GetComponent<Collider>();
        }

        void OnEnable() {
            currentHealth = maxHealth;
            enemyCollider.enabled = true;
            aiTree.SetActive(true);
        }

        public void TakeDamage(int damage, WeaponDamageType? damageType) {
            HitFlash();

            if (hasShield) {
                BreakShield();
                return;
            }

            var dam = damage;
            if (damageType == weakness) {
                dam += (int)Math.Floor(dam * weaknessFactor / 100.0f);
            }
            else if (damageType == resistance) {
                dam -= (int)Math.Floor(dam * resistanceFactor / 100.0f);
            }

            if (damageType != null) {
                PlayHitSound(damageType.Value);
            }

            currentHealth -= dam;
            healthBar.SetProgress((float)currentHealth / maxHealth);
            if (currentHealth <= 0) Kill();
        }

        void PlayHitSound(WeaponDamageType damageType) {
            switch (damageType) {
                case WeaponDamageType.Spear:
                    SoundFXManager.PlayRandom(spearHitSounds);
                    break;
                case WeaponDamageType.Sword:
                    SoundFXManager.PlayRandom(swordHitSounds);
                    break;
                case WeaponDamageType.Hammer:
                    SoundFXManager.PlayRandom(hammerHitSounds);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(damageType), damageType, null);
            }
        }

        public void Kill() {
            GameManager.Instance.AddKillCount(type);
            var soul = Instantiate(Resources.Load("Level/Prefabs/VFX/Soul"), transform.position + Vector3.up, Quaternion.identity) as GameObject;
            soul.GetComponent<SoulVFX>().soulType = type;

            KillWithoutSoul();
        }

        public void KillWithoutSoul() {
            enemyMainGameObject.GetComponent<AITree>().SetActive(false);
            StartCoroutine(Disolve());
        }

        public void Heal(int heal) {
            int newHealth = currentHealth + heal;
            int overheal  = newHealth     - maxHealth;
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
            
            skinnedMeshes.ForEach(skinnedMesh => skinnedMesh.materials[1].SetFloat(ShaderPropertyShieldAmount, 0.5f));
        }

        public void BreakShield() {
            hasShield = false;
            if (meshes == null) return;

            skinnedMeshes.ForEach(skinnedMesh => skinnedMesh.materials[1].SetFloat(ShaderPropertyShieldAmount, 0));
        }

        void HitFlash() {
            if (meshes == null) return;
            skinnedMeshes.ForEach(skinnedMesh => skinnedMesh.materials[1].SetFloat(ShaderPropertyShieldAmount, 1));

            StartCoroutine(ResetHitFlash());
        }

        IEnumerator ResetHitFlash() {
            yield return new WaitForSeconds(0.05f);
            skinnedMeshes.ForEach(skinnedMesh => skinnedMesh.materials[1].SetFloat(ShaderPropertyShieldAmount, 0));
        }

        void OnDisable() {
            enemyMainGameObject.SetActive(false);
        }

        IEnumerator Disolve() {
            enemyCollider.enabled = false;
            smokeVFX.Play();

            float time = 2f;
            while (time >= 0) {
                float prog = 1 - time / 2f;
                skinnedMeshes.ForEach(skinnedMesh => skinnedMesh.materials[1].SetFloat(ShaderPropertyShieldAmount, prog));

                time -= Time.deltaTime;
                yield return null;
            }

            skinnedMeshes.ForEach(skinnedMesh => skinnedMesh.materials[1].SetFloat(ShaderPropertyShieldAmount, 0));

            enemyMainGameObject.SetActive(false);
            SpawnManager.Instance.EnqueueEnemy(enemyMainGameObject);
        }
    }
}