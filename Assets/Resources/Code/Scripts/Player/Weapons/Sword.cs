using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Sword", menuName = "ExodusTools/Weapon/Sword")]
public class Sword : Weapon {
    PlayerMovement playerMovement;
    BoxCollider swordCollider;
    LayerMask enemyLayer = -1;
    SwordAbility swordAbility;

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

        UppercutEnemies(player);

        animator.SetTrigger("SwordSpecial");
        playerMovement.Jump();
    }

    public override void WeakpointAttack(Animator animator, GameObject player, Weakpoint weakpoint, Vector3 hitLocation) {
        weakpoint.TakeDamage(baseDamage, WeaponDamageType.SWORD, hitLocation);
        BasicAttack(animator, player);
    }

    void UppercutEnemies(GameObject player) {
        if (enemyLayer < 0) enemyLayer = LayerMask.NameToLayer("Enemy");
        if (swordCollider == null) swordCollider = Camera.main.GetComponent<BoxCollider>();
        if (swordAbility == null) swordAbility = player.GetComponent<SwordAbility>();

        Collider[] hitEnemies = Physics.OverlapBox(
            swordCollider.bounds.center,
            swordCollider.bounds.extents,
            swordCollider.transform.rotation
        );

        foreach (Collider enemy in hitEnemies) {
            if (enemy.gameObject.layer == enemyLayer) {
                // Debug.Log("Hit " + enemy.name);
                enemy.GetComponent<NavMeshAgent>().isStopped = true;
                enemy.GetComponent<NavMeshAgent>().updatePosition = false;
                // enemy.GetComponent<NavMeshAgent>().enabled = false;
                enemy.GetComponent<Rigidbody>().AddForce(Vector3.up * 20, ForceMode.Impulse);
                // swordAbility.Uppercut(enemy);
            }
        }
    }
}
