using System;
using System.Collections;
using LexUtils.Events;
using TEE.Player.Camera;
using TEE.Player.Movement;
using TEE.UI;
using UnityEngine;

namespace TEE.Health {
    public enum ShieldLevel {
        None,
        Cracked,
        LevelOne,
        LevelTwo,
    }

    public class PlayerHealthSystem : MonoBehaviour {
        [SerializeField] int                      maxHealth    = 100;
        [SerializeField] ShieldLevel              shieldStatus = ShieldLevel.None;
        [SerializeField] PlayerShieldUIController playerShieldUIController;
        [SerializeField] int                      currentHealth;
        [SerializeField] int                      layerOneShieldSpeed = 10;
        [SerializeField] int                      layerTwoShieldSpeed = 20;
        [SerializeField] int                      timeForShield       = 5;
        [SerializeField] int                      shieldCooldown      = 5;
        [SerializeField] float                    shieldGatingTime    = 1f;

        Rigidbody rb;

        bool canShieldGate   = true;
        bool canGetShieldOne = true;
        bool canGetShieldTwo;
        bool shieldGatingActive;
        bool dead;

        void Start() {
            currentHealth     = maxHealth;
            rb                = Player.Movement.Player.Rigidbody;
        }

        public void TakeDamage(int damage) {
            if (dead) return;
            switch (shieldStatus) {
                case ShieldLevel.Cracked:
                case ShieldLevel.LevelOne:
                    BreakShield();
                    break;
                case ShieldLevel.LevelTwo:
                    DamageShield();
                    break;
                case ShieldLevel.None:
                    DamagePlayer(damage);
                    break;
                default:
                    Debug.LogError("Invalid shield status");
                    break;
            }
        }

        void FixedUpdate() {
            if (canGetShieldOne) StartCoroutine(TryGetShieldOne());
            if (canGetShieldTwo) StartCoroutine(TryGetShieldTwo());
        }

        IEnumerator TryGetShieldOne() {
            canGetShieldOne = false;
            float time   = 0;
            bool  failed = false;
            while (!failed && time < timeForShield) {
                if (rb.velocity.magnitude < layerOneShieldSpeed) {
                    failed = true;
                }

                time += Time.fixedDeltaTime;
                yield return null;
            }

            if (!failed) {
                ShieldOne();
                canGetShieldTwo = true;
            }
            else StartCoroutine(ResetShieldCooldown());
        }

        IEnumerator TryGetShieldTwo() {
            canGetShieldTwo = false;
            float time   = 0;
            bool  failed = false;
            while (!failed && time < timeForShield) {
                if (rb.velocity.magnitude < layerTwoShieldSpeed) {
                    failed = true;
                }

                time += Time.fixedDeltaTime;
                yield return null;
            }

            if (!failed) ShieldTwo();
            else StartCoroutine(ResetShieldCooldown());
        }

        IEnumerator ResetShieldCooldown() {
            yield return new WaitForSeconds(shieldCooldown);
            canGetShieldOne = true;
        }

        public void Kill() {
            dead              = true;
            rb.freezeRotation = false;
            GameManager.Instance.Kill();
        }

        public void Heal(int heal) {
            int newHealth = currentHealth + heal;
            int overheal  = newHealth     - maxHealth;
            if (overheal > 0) Overheal(overheal);

            currentHealth = Math.Min(maxHealth, newHealth);
            HealthbarController.Instance.SetProgress((float)currentHealth / maxHealth);
        }

        void Overheal(int overheal) {
            Debug.Log("Overheal: " + overheal);
        }

        void ShieldTwo() {
            shieldStatus = ShieldLevel.LevelTwo;
            playerShieldUIController.ShieldTwo();
            HealthbarController.Instance.SetColor(2);
        }

        void ShieldOne() {
            shieldStatus = ShieldLevel.LevelOne;
            playerShieldUIController.ShieldOne();
            HealthbarController.Instance.SetColor(1);
        }

        void DamagePlayer(int damage) {
            if (shieldGatingActive) return;
            EventForge.Float.Get("Player.Trauma").Invoke(0.2f);
            currentHealth -= damage;
            HealthbarController.Instance.InduceStress(0.2f);
            HealthbarController.Instance.SetProgress((float)currentHealth / maxHealth);
            if (currentHealth <= 0) Kill();
        }

        void DamageShield() {
            EventForge.Float.Get("Player.Trauma").Invoke(0.1f);
            shieldStatus    = ShieldLevel.Cracked;
            canGetShieldTwo = true;
            playerShieldUIController.DamageShield();
            HealthbarController.Instance.SetColor(0);
            HealthbarController.Instance.InduceStress(0.5f);

            if (!canShieldGate) return;
            canShieldGate      = false;
            shieldGatingActive = true;
            StartCoroutine(ResetShieldGating());
        }

        void BreakShield() {
            EventForge.Float.Get("Player.Trauma").Invoke(0.3f);
            if (shieldGatingActive) return;
            shieldStatus = ShieldLevel.None;
            playerShieldUIController.BreakShield();
            HealthbarController.Instance.SetColor(0);
            HealthbarController.Instance.InduceStress(1f);
            StartCoroutine(ResetShieldCooldown());

            if (!canShieldGate) return;
            canShieldGate      = false;
            shieldGatingActive = true;
            StartCoroutine(ResetShieldGating());
        }

        IEnumerator ResetShieldGating() {
            yield return new WaitForSeconds(shieldGatingTime);
            canShieldGate      = true;
            shieldGatingActive = false;
        }
    }
}