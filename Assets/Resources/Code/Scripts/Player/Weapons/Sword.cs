using UnityEngine;

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
        healthSystem.TakeDamage(baseDamage, WeaponDamageType.SWORD, hitLocation);
        BasicAttack(animator, player);
    }

    public override void SpecialAttack(Animator animator, GameObject player, HealthSystem healthSystem) {
        if (playerMovement == null) {
            playerMovement = player.GetComponent<PlayerMovement>();
        }

        UppercutEnemies();

        animator.SetTrigger("SwordSpecial");
        playerMovement.Jump();
    }

    void UppercutEnemies() {
        if(enemyLayer < 0) enemyLayer = LayerMask.NameToLayer("Enemy");
        if(swordCollider == null) swordCollider = Camera.main.GetComponent<BoxCollider>();
        Debug.Log(swordCollider);

        Collider[] hitEnemies = Physics.OverlapBox(
            swordCollider.transform.position + swordCollider.bounds.center,
            swordCollider.bounds.extents,
            swordCollider.transform.rotation
        );

        foreach (Collider enemy in hitEnemies) {
            Debug.Log("Hit " + enemy.name);
            if (enemy.gameObject.layer == enemyLayer) {
                enemy.GetComponent<Rigidbody>().velocity = Vector3.zero;
                enemy.GetComponent<Rigidbody>().AddForce(Vector3.up * 50, ForceMode.Impulse);
            }
        }        
    }
}
