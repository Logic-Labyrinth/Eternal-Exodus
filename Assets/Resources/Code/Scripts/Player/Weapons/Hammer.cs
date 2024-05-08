using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Hammer", menuName = "ExodusTools/Weapon/Hammer")]
public class Hammer : Weapon {
    private HammerAbility hammer;
    LayerMask enemyLayer = -1;
    List<GameObject> hammerTargets;

    public override void BasicAttack(Animator animator, GameObject player) {
        animator.SetTrigger("HammerAttack");
        PlayBasicAttackSound();
    }

    public override void BasicAttack(Animator animator, GameObject player, HealthSystem healthSystem, Vector3 hitLocation) {
        if (enemyLayer < 0) enemyLayer = LayerMask.NameToLayer("Enemy");
        BasicAttack(animator, player);

        hammerTargets = CustomConeCollider.GetAllObjects(
            Camera.main.transform,
            attackRange,    // radius
            30,             // vertical angle
            90              // horizontal angle
        );

        foreach (GameObject target in hammerTargets) {
            if (target.layer == enemyLayer)
                target.GetComponent<HealthSystem>().TakeDamage(baseDamage, WeaponDamageType.HAMMER, Vector3.zero);
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
        hammer.ActivateHammerAbility(baseDamage, attackRange);
    }

    public override void Reset() {
        if(hammer == null) return;
        hammer.Reset();
    }
}
