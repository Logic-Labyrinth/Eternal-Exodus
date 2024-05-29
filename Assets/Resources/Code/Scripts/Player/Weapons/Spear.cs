using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spear", menuName = "ExodusTools/Weapon/Spear")]
public class Spear : Weapon {
    PlayerDashing playerDash;
    LayerMask enemyLayer = -1;
    List<GameObject> spearTargets;

    public override void BasicAttack(Animator animator) {
        if (enemyLayer < 0) enemyLayer = LayerMask.NameToLayer("Enemy");
        animator.SetTrigger("SpearAttack");
        PlayBasicAttackSound();

        spearTargets = CustomCapsuleCollider.GetAllObjects(
            Camera.main.transform,
            Camera.main.transform.forward,
            1,
            attackRange
        );

        foreach (GameObject target in spearTargets) {
            if (target.layer == enemyLayer) target.GetComponent<HealthSystem>().TakeDamage(baseDamage, WeaponDamageType.SPEAR);
            if (target.CompareTag("Breakable")) target.GetComponent<BreakableObject>().Break();
        }
    }

    public override void SpecialAttack(Animator animator, GameObject player) {
        if (playerDash == null) playerDash = player.GetComponent<PlayerDashing>();

        animator.SetTrigger("SpearSpecial");
        playerDash.Dash();
        PlaySpecialAttackSound();
    }

    public override void WeakpointAttack(Animator animator, Weakpoint weakpoint) {
        weakpoint.TakeDamage(baseDamage, WeaponDamageType.SPEAR);
        BasicAttack(animator);
    }
}
