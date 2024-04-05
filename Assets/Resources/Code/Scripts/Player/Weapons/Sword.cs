using UnityEngine;

[CreateAssetMenu(fileName = "Sword", menuName = "ExodusTools/Weapon/Sword")]
public class Sword : Weapon {
    PlayerMovement playerMovement;

    public override void BasicAttack(Animator animator, GameObject player) {
        animator.SetTrigger("SwordAttack");
        PlayBasicAttackSound();
    }

    public override void BasicAttack(Animator animator, GameObject player, HealthSystem healthSystem, Vector3 hitLocation) {
        healthSystem.TakeDamage(baseDamage, WeaponDamageType.SWORD, hitLocation);
        BasicAttack(animator, player);
    }

    public override void SpecialAttack(Animator animator, GameObject player, HealthSystem healthSystem) {
        if (playerMovement == null) {
            playerMovement = player.GetComponent<PlayerMovement>();
        }

        animator.SetTrigger("SwordSpecial");
        playerMovement.Jump();
    }
}
