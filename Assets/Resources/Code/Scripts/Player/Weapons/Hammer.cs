using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Hammer", menuName = "ExodusTools/Weapon/Hammer")]
public class Hammer : Weapon {
    private HammerAbility hammer;
    LayerMask enemyLayer = -1;
    List<GameObject> hammerTargets;

    public override void BasicAttack(Animator animator) {
        if (enemyLayer < 0) enemyLayer = LayerMask.NameToLayer("Enemy");
        animator.SetTrigger("HammerAttack");
        PlayBasicAttackSound();

        hammerTargets = CustomTriggers.ArcRaycast(Camera.main.transform, 120, attackRange, 20);

        foreach (GameObject target in hammerTargets) {
            if (target.layer == enemyLayer) target.GetComponent<HealthSystem>().TakeDamage(baseDamage, WeaponDamageType.HAMMER);
            if (target.CompareTag("Breakable")) target.GetComponent<BreakableObject>().Break();
        }
    }

    public override void SpecialAttack(Animator animator, GameObject player) {
        if (hammer == null) {
            hammer = player.GetComponent<HammerAbility>();
        }

        animator.ResetTrigger("SwapHammer");
        animator.ResetTrigger("HammerRelease");
        animator.SetTrigger("HammerCharge");
        hammer.ChargeHammer();
        PlaySpecialAttackSound();
    }

    public override void WeakpointAttack(Animator animator, Weakpoint weakpoint) {
        weakpoint.TakeDamage(baseDamage, WeaponDamageType.HAMMER);
        BasicAttack(animator);
    }

    public override void SpecialRelease(Animator animator, GameObject player) {
        if (hammer == null) {
            hammer = player.GetComponent<HammerAbility>();
        }

        animator.SetTrigger("HammerRelease");
        hammer.ActivateHammerAbility(baseDamage, attackRange, this);
    }

    public override void Reset() {
        if (hammer == null) return;
        hammer.Reset();
    }
}
