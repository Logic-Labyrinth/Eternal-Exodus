using System;
using System.Collections;
using TEE.AI;
using TEE.Audio;
using TEE.Enemies;
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

        int                 currentHealth;
        static readonly int ShaderPropertyShieldAmount   = Shader.PropertyToID("_ShieldAmount");
        static readonly int ShaderPropertyHitFlashBool   = Shader.PropertyToID("_HitFlashBool");
        static readonly int ShaderPropertyDissolveAmount = Shader.PropertyToID("_Dissolve_Amount");

        void OnEnable() {
            currentHealth = maxHealth;
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
                case WeaponDamageType.Spear:
                    SoundFXManager.Instance.PlayRandom(spearHitSounds);
                    break;
                case WeaponDamageType.Sword:
                    SoundFXManager.Instance.PlayRandom(swordHitSounds);
                    break;
                case WeaponDamageType.Hammer:
                    SoundFXManager.Instance.PlayRandom(hammerHitSounds);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(damageType), damageType, null);
            }
        }

        public void Kill() {
            GameManager.Instance.AddKillCount(type);
            var soul = Instantiate(Resources.Load("Level/Prefabs/VFX/Soul"), transform.position + Vector3.up, Quaternion.identity) as GameObject;
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
            foreach (var mesh in meshes) {
                mesh.GetComponent<SkinnedMeshRenderer>().materials[1].SetFloat(ShaderPropertyShieldAmount, 0.5f);
            }
        }

        public void BreakShield() {
            hasShield = false;
            if (meshes == null) return;

            foreach (var mesh in meshes) {
                mesh.GetComponent<SkinnedMeshRenderer>().materials[1].SetFloat(ShaderPropertyShieldAmount, 0);
            }
        }

        void HitFlash() {
            if (meshes == null) return;
            foreach (var mesh in meshes) {
                mesh.GetComponent<SkinnedMeshRenderer>().materials[0].SetInt(ShaderPropertyHitFlashBool, 1);
            }

            StartCoroutine(ResetHitFlash());
        }

        IEnumerator ResetHitFlash() {
            yield return new WaitForSeconds(0.05f);
            foreach (var mesh in meshes) {
                mesh.GetComponent<SkinnedMeshRenderer>().materials[0].SetInt(ShaderPropertyHitFlashBool, 0);
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
                foreach (var mesh in meshes) {
                    mesh.GetComponent<SkinnedMeshRenderer>().materials[0].SetFloat(ShaderPropertyDissolveAmount, prog);
                }

                time -= Time.deltaTime;
                yield return null;
            }

            foreach (var mesh in meshes) {
                mesh.GetComponent<SkinnedMeshRenderer>().materials[0].SetFloat(ShaderPropertyDissolveAmount, 0f);
            }

            enemyMainGameObject.GetComponent<AITree>().SetActive(true);
            enemyMainGameObject.SetActive(false);
            GetComponent<Collider>().enabled = true;
            SpawnManager.Instance.EnqueueEnemy(enemyMainGameObject);
        }
    }
}