using System;
using System.Collections;
using UnityEngine;

public enum ShieldLevel {
    NONE,
    CRACKED,
    LEVEL_ONE,
    LEVEL_TWO,
}

public class PlayerHealthSystem : MonoBehaviour {
    [SerializeField] int maxHealth = 100;
    [SerializeField] ShieldLevel shieldStatus = ShieldLevel.NONE;
    [SerializeField] PlayerShieldUIController playerShieldUIController;
    [SerializeField] int currentHealth;
    [SerializeField] int layerOneShieldSpeed = 10;
    [SerializeField] int layerTwoShieldSpeed = 20;
    [SerializeField] int timeForShield = 5;
    [SerializeField] int shieldCooldown = 5;
    [SerializeField] float shieldGatingTime = 1f;

    PlayerMovement pm;
    Rigidbody rb;
    bool canGetShieldOne = true;
    bool canGetShieldTwo = false;
    bool shieldGatingActive = false;
    bool canShieldGate = true;
    bool dead = false;

    void Awake() {
        currentHealth = maxHealth;
        pm = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    public void TakeDamage(int damage) {
        if (dead) return;
        switch (shieldStatus) {
            case ShieldLevel.CRACKED:
            case ShieldLevel.LEVEL_ONE:
                BreakShield();
                break;
            case ShieldLevel.LEVEL_TWO:
                DamageShield();
                break;
            case ShieldLevel.NONE:
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
        float time = 0;
        bool failed = false;
        while (!failed && time < timeForShield) {
            if (pm.rb.velocity.magnitude < layerOneShieldSpeed) {
                failed = true;
            }
            time += Time.fixedDeltaTime;
            yield return null;
        }

        if (!failed) {
            ShieldOne();
            canGetShieldTwo = true;
        } else StartCoroutine(ResetShieldCooldown());
    }

    IEnumerator TryGetShieldTwo() {
        canGetShieldTwo = false;
        float time = 0;
        bool failed = false;
        while (!failed && time < timeForShield) {
            if (pm.rb.velocity.magnitude < layerTwoShieldSpeed) {
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
        dead = true;
        rb.freezeRotation = false;
        GameManager.Instance.Kill();
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

    public void ShieldTwo() {
        shieldStatus = ShieldLevel.LEVEL_TWO;
        playerShieldUIController.ShieldTwo();
    }

    public void ShieldOne() {
        shieldStatus = ShieldLevel.LEVEL_ONE;
        playerShieldUIController.ShieldOne();
    }

    void DamagePlayer(int damage) {
        if (shieldGatingActive) return;
        CameraPositioning.Instance.InduceStress(0.2f);
        currentHealth -= damage;
        if (currentHealth <= 0) Kill();
    }

    public void DamageShield() {
        CameraPositioning.Instance.InduceStress(0.1f);
        shieldStatus = ShieldLevel.CRACKED;
        canGetShieldTwo = true;
        playerShieldUIController.DamageShield();

        if (canShieldGate) {
            canShieldGate = false;
            shieldGatingActive = true;
            StartCoroutine(ResetShieldGating());
        }
    }

    public void BreakShield() {
        CameraPositioning.Instance.InduceStress(0.3f);
        if (shieldGatingActive) return;
        shieldStatus = ShieldLevel.NONE;
        playerShieldUIController.BreakShield();
        StartCoroutine(ResetShieldCooldown());

        if (canShieldGate) {
            canShieldGate = false;
            shieldGatingActive = true;
            StartCoroutine(ResetShieldGating());
        }
    }

    IEnumerator ResetShieldGating() {
        yield return new WaitForSeconds(shieldGatingTime);
        canShieldGate = true;
        shieldGatingActive = false;
    }
}
