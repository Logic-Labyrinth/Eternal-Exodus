using System;
using UnityEngine;

public enum ShieldStatus {
    NONE,
    DAMAGED,
    FULL
}

public class PlayerHealthSystem : MonoBehaviour {
    [SerializeField] int maxHealth = 100;
    [SerializeField] ShieldStatus shieldStatus = ShieldStatus.NONE;
    // [SerializeField] ShieldStatus shieldStatus = ShieldStatus.FULL;
    [SerializeField] PlayerShieldUIController playerShieldUIController;

    int currentHealth;

    void Awake() {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage) {
        switch (shieldStatus) {
            case ShieldStatus.DAMAGED:
                BreakShield();
                return;
            case ShieldStatus.FULL:
                DamageShield();
                return;
            case ShieldStatus.NONE:
                DamagePlayer(damage);
                break;
            default:
                Debug.LogError("Invalid shield status");
                break;
        }
    }

    // private void Update() {
    //     if(Input.GetKeyDown(KeyCode.Z)) Shield();
    //     if(Input.GetKeyDown(KeyCode.X)) DamageShield();
    //     if(Input.GetKeyDown(KeyCode.C)) BreakShield();
    // }

    public void Kill() {
        // gameObject.SetActive(false);
        Debug.Log("Player died");
    }

    public void Heal(int heal) {
        int newHealth = currentHealth + heal;
        int overheal = newHealth - maxHealth;
        if (overheal > 0) Overheal(overheal);

        currentHealth = Math.Min(maxHealth, newHealth);
    }

    void DamagePlayer(int damage) {
        Debug.Log("Damage: " + damage);

        currentHealth -= damage;
        if (currentHealth <= 0) Kill();
    }

    void Overheal(int overheal) {
        Debug.Log("Overheal: " + overheal);
    }

    public void Shield() {
        shieldStatus = ShieldStatus.FULL;
        playerShieldUIController.Shield();
    }

    public void DamageShield() {
        shieldStatus = ShieldStatus.DAMAGED;
        playerShieldUIController.DamageShield();
    }

    public void BreakShield() {
        shieldStatus = ShieldStatus.NONE;
        playerShieldUIController.BreakShield();
    }
}
