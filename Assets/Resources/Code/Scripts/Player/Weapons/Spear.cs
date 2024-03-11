using UnityEngine;

[CreateAssetMenu(fileName = "Spear", menuName = "ExodusTools/Weapon/Spear")]
public class Spear : Weapon {
    private PlayerDashing playerDash;

    public override void BasicAttack(GameObject player, HealthSystem healthSystem, Vector3 hitLocation) {
        // Basic attack logic
        healthSystem.TakeDamage(baseDamage, WeaponDamageType.SPEAR, hitLocation);
    }

    public override void SpecialAttack(GameObject player, HealthSystem healthSystem) {
        if (playerDash == null) {
            playerDash = player.GetComponent<PlayerDashing>();
        }
        playerDash.Dash();
    }
}