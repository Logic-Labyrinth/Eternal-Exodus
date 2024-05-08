using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Sword", menuName = "ExodusTools/Weapon/Sword")]
public class Sword : Weapon {
    PlayerMovement playerMovement;
    LayerMask enemyLayer = -1;
    List<GameObject> swordTargets;

    public override void BasicAttack(Animator animator, GameObject player) {
        animator.SetTrigger("SwordAttack");
        PlayBasicAttackSound();
    }

    public override void BasicAttack(Animator animator, GameObject player, HealthSystem healthSystem, Vector3 hitLocation) {
        if (enemyLayer < 0) enemyLayer = LayerMask.NameToLayer("Enemy");
        BasicAttack(animator, player);

        swordTargets = CustomConeCollider.GetAllObjects(
            Camera.main.transform,
            attackRange,    // radius
            30,             // vertical angle
            90              // horizontal angle
        );

        foreach (GameObject target in swordTargets) {
            if (target.layer == enemyLayer)
                target.GetComponent<HealthSystem>().TakeDamage(baseDamage, WeaponDamageType.SWORD, Vector3.zero);
        }
    }

    public override void SpecialAttack(Animator animator, GameObject player, HealthSystem healthSystem) {
        if (playerMovement == null) playerMovement = player.GetComponent<PlayerMovement>();

        UppercutEnemies();

        animator.SetTrigger("SwordSpecial");
        playerMovement.SwordJump();
        PlaySpecialAttackSound();
    }

    public override void WeakpointAttack(Animator animator, GameObject player, Weakpoint weakpoint, Vector3 hitLocation) {
        weakpoint.TakeDamage(baseDamage, WeaponDamageType.SWORD, hitLocation);
        BasicAttack(animator, player);
    }

    void UppercutEnemies() {
        if (enemyLayer < 0) enemyLayer = LayerMask.NameToLayer("Enemy");

        swordTargets = CustomConeCollider.GetAllObjects(
            Camera.main.transform,
            attackRange,    // radius
            30,             // vertical angle
            90              // horizontal angle
        );

        foreach (GameObject target in swordTargets) {
            if (target.layer == enemyLayer) {
                target.GetComponent<HealthSystem>().TakeDamage(baseDamage, WeaponDamageType.SWORD, Vector3.zero);
                if (!target.TryGetComponent(out Rigidbody r)) continue;
                target.GetComponent<NavMeshAgent>().isStopped = true;
                target.GetComponent<NavMeshAgent>().updatePosition = false;
                target.GetComponent<Rigidbody>().AddForce(Vector3.up * 20, ForceMode.Impulse);
            }
        }
    }
}
