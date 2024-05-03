using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Sword", menuName = "ExodusTools/Weapon/Sword")]
public class Sword : Weapon {
    PlayerMovement playerMovement;
    BoxCollider swordCollider;
    LayerMask enemyLayer = -1;

    public override void BasicAttack(Animator animator, GameObject player) {
        animator.SetTrigger("SwordAttack");
        PlayBasicAttackSound();
    }

    public override void BasicAttack(Animator animator, GameObject player, HealthSystem healthSystem, Vector3 hitLocation) {
        // healthSystem.TakeDamage(baseDamage, WeaponDamageType.SWORD, hitLocation);
        BasicAttack(animator, player);

        if (swordCollider == null) swordCollider = Camera.main.GetComponent<BoxCollider>();
        Collider[] hitEnemies = Physics.OverlapBox(
            swordCollider.bounds.center,
            swordCollider.bounds.extents,
            swordCollider.transform.rotation
        );

        foreach (Collider enemy in hitEnemies) {
            if (enemy.gameObject.layer == enemyLayer) {
                enemy.GetComponent<HealthSystem>().TakeDamage(baseDamage, WeaponDamageType.SWORD, enemy.transform.position);
            }
        }
    }

    public override void SpecialAttack(Animator animator, GameObject player, HealthSystem healthSystem) {
        if (playerMovement == null) {
            playerMovement = player.GetComponent<PlayerMovement>();
        }

        UppercutEnemies(player);

        animator.SetTrigger("SwordSpecial");
        playerMovement.SwordJump();
        PlaySpecialAttackSound();
    }

    public override void WeakpointAttack(Animator animator, GameObject player, Weakpoint weakpoint, Vector3 hitLocation) {
        weakpoint.TakeDamage(baseDamage, WeaponDamageType.SWORD, hitLocation);
        BasicAttack(animator, player);
    }

    void UppercutEnemies(GameObject player) {
        if (enemyLayer < 0) enemyLayer = LayerMask.NameToLayer("Enemy");
        if (swordCollider == null) swordCollider = Camera.main.GetComponent<BoxCollider>();

        Collider[] hitEnemies = Physics.OverlapBox(
            swordCollider.bounds.center,
            swordCollider.bounds.extents,
            swordCollider.transform.rotation
        );

        foreach (Collider enemy in hitEnemies) {
            if (enemy.gameObject.layer == enemyLayer) {
                if (!enemy.TryGetComponent(out Rigidbody r)) continue;
                enemy.GetComponent<NavMeshAgent>().isStopped = true;
                enemy.GetComponent<NavMeshAgent>().updatePosition = false;
                enemy.GetComponent<Rigidbody>().AddForce(Vector3.up * 20, ForceMode.Impulse);
                enemy.GetComponent<HealthSystem>().TakeDamage(baseDamage, WeaponDamageType.SWORD, enemy.transform.position);
            }
        }
    }
}
