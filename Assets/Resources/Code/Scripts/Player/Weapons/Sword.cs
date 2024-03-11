using UnityEngine;

[CreateAssetMenu(fileName = "Sword", menuName = "ExodusTools/Weapon/Sword")]
public class Sword : Weapon {
    private PlayerMovement playerMovement;

    public override void BasicAttack(GameObject player, HealthSystem healthSystem) {
        // Basic attack logic
        healthSystem.TakeDamage(baseDamage, WeaponDamageType.SWORD);
    }

    public override void SpecialAttack(GameObject player, HealthSystem healthSystem) {
        if (playerMovement == null) {
            playerMovement = player.GetComponent<PlayerMovement>();
        }

        playerMovement.Jump();
    }
}
