using UnityEngine;

[CreateAssetMenu(fileName = "Hammer", menuName = "ExodusTools/Weapon/Hammer")]
public class Hammer : Weapon {
    private HammerAbility hammer;
    BoxCollider hammerCollider;
    LayerMask enemyLayer = -1;

    public override void BasicAttack(Animator animator, GameObject player) {
        animator.SetTrigger("HammerAttack");
        PlayBasicAttackSound();
    }

    public override void BasicAttack(Animator animator, GameObject player, HealthSystem healthSystem, Vector3 hitLocation) {
        if (enemyLayer < 0) enemyLayer = LayerMask.NameToLayer("Enemy");
        BasicAttack(animator, player);
        // healthSystem.TakeDamage(baseDamage, WeaponDamageType.HAMMER, hitLocation);

        if (hammerCollider == null) hammerCollider = Camera.main.GetComponent<BoxCollider>();
        Collider[] hitEnemies = Physics.OverlapBox(
            hammerCollider.bounds.center,
            hammerCollider.bounds.extents,
            hammerCollider.transform.rotation
        );

        foreach (Collider enemy in hitEnemies) {
            if (enemy.gameObject.layer == enemyLayer) {
                enemy.GetComponent<HealthSystem>().TakeDamage(baseDamage, WeaponDamageType.SWORD, enemy.transform.position);
            }
        }
    }

    public override void SpecialAttack(Animator animator, GameObject player, HealthSystem healthSystem) {
        if (hammer == null) {
            hammer = player.GetComponent<HammerAbility>();
        }

        animator.ResetTrigger("SwapHammer");
        animator.SetTrigger("HammerCharge");
        hammer.ChargeHammer();
        PlaySpecialAttackSound();
    }

    public override void WeakpointAttack(Animator animator, GameObject player, Weakpoint weakpoint, Vector3 hitLocation) {
        weakpoint.TakeDamage(baseDamage, WeaponDamageType.HAMMER, hitLocation);
        BasicAttack(animator, player);
    }

    public override void SpecialRelease(Animator animator, GameObject player, HealthSystem healthSystem) {
        if (hammer == null) {
            hammer = player.GetComponent<HammerAbility>();
        }

        animator.SetTrigger("HammerRelease");
        hammer.ActivateHammerAbility(baseDamage);
    }

    public override void Reset() {
        if(hammer == null) return;
        hammer.Reset();
    }
}
