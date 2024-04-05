using UnityEngine;

[CreateAssetMenu(fileName = "Spear", menuName = "ExodusTools/Weapon/Spear")]
public class Spear : Weapon {
    private PlayerDashing playerDash;

    public override void BasicAttack(Animator animator, GameObject player) {
        animator.SetTrigger("SpearAttack");
        PlayBasicAttackSound();
    }

    public override void BasicAttack(Animator animator, GameObject player, HealthSystem healthSystem, Vector3 hitLocation) {
        // Basic attack logic
        healthSystem.TakeDamage(baseDamage, WeaponDamageType.SPEAR, hitLocation);
        BasicAttack(animator, player);
    }

    public override void SpecialAttack(Animator animator, GameObject player, HealthSystem healthSystem) {
        if (playerDash == null) {
            playerDash = player.GetComponent<PlayerDashing>();
        }
        
        animator.SetTrigger("SpearSpecial");
        playerDash.Dash();
    }
}
