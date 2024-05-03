using UnityEngine;

[CreateAssetMenu(fileName = "Spear", menuName = "ExodusTools/Weapon/Spear")]
public class Spear : Weapon {
    PlayerDashing playerDash;

    public override void BasicAttack(Animator animator, GameObject player) {
        animator.SetTrigger("SpearAttack");
        PlayBasicAttackSound();
    }

    public override void BasicAttack(Animator animator, GameObject player, HealthSystem healthSystem, Vector3 hitLocation) {
        healthSystem.TakeDamage(baseDamage, WeaponDamageType.SPEAR, hitLocation);
        BasicAttack(animator, player);
    }

    public override void SpecialAttack(Animator animator, GameObject player, HealthSystem healthSystem) {
        if (playerDash == null) {
            playerDash = player.GetComponent<PlayerDashing>();
        }

        animator.SetTrigger("SpearSpecial");
        playerDash.Dash();
        PlaySpecialAttackSound();
    }

    public override void WeakpointAttack(Animator animator, GameObject player, Weakpoint weakpoint, Vector3 hitLocation) {
        weakpoint.TakeDamage(baseDamage, WeaponDamageType.SPEAR, hitLocation);
        BasicAttack(animator, player);
    }
}
