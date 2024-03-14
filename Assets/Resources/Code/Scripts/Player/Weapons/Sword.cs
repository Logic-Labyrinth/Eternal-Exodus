using UnityEngine;

[CreateAssetMenu(fileName = "Sword", menuName = "ExodusTools/Weapon/Sword")]
public class Sword : Weapon {
    private PlayerMovement playerMovement;

    public override void BasicAttack(GameObject player, HealthSystem healthSystem, Vector3 hitLocation) {
        // Basic attack logic
        healthSystem.TakeDamage(baseDamage, WeaponDamageType.SWORD, hitLocation);
    }

    public override void SpecialAttack(GameObject player, HealthSystem healthSystem) {
        if (playerMovement == null) {
            playerMovement = player.GetComponent<PlayerMovement>();
        }

        playerMovement.Jump();
    }
}
