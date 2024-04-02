using UnityEngine;

[CreateAssetMenu(fileName = "Sword", menuName = "ExodusTools/Weapon/Sword")]
public class Sword : Weapon {
    PlayerMovement playerMovement;

    public override void BasicAttack(GameObject player) {
        PlayBasicAttackSound();
    }

    public override void BasicAttack(GameObject player, HealthSystem healthSystem, Vector3 hitLocation) {
        healthSystem.TakeDamage(baseDamage, WeaponDamageType.SWORD, hitLocation);
        BasicAttack(player);
    }

    public override void SpecialAttack(GameObject player, HealthSystem healthSystem) {
        if (playerMovement == null) {
            playerMovement = player.GetComponent<PlayerMovement>();
        }

        playerMovement.Jump();
    }
}
