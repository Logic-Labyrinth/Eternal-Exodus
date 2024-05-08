using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spear", menuName = "ExodusTools/Weapon/Spear")]
public class Spear : Weapon {
    PlayerDashing playerDash;
    LayerMask enemyLayer = -1;
    List<GameObject> spearTargets;

    public override void BasicAttack(Animator animator, GameObject player) {
        animator.SetTrigger("SpearAttack");
        PlayBasicAttackSound();
    }

    public override void BasicAttack(Animator animator, GameObject player, HealthSystem healthSystem, Vector3 hitLocation) {
        if (enemyLayer < 0) enemyLayer = LayerMask.NameToLayer("Enemy");
        BasicAttack(animator, player);

        spearTargets = CustomCapsuleCollider.GetAllObjects(
            Camera.main.transform,
            Camera.main.transform.forward,
            1,
            attackRange
        );

        foreach (GameObject target in spearTargets) {
            if (target.layer == enemyLayer)
                target.GetComponent<HealthSystem>().TakeDamage(baseDamage, WeaponDamageType.SPEAR, Vector3.zero);
        }
    }

    public override void SpecialAttack(Animator animator, GameObject player, HealthSystem healthSystem) {
        if (playerDash == null) playerDash = player.GetComponent<PlayerDashing>();

        animator.SetTrigger("SpearSpecial");
        playerDash.Dash();
        PlaySpecialAttackSound();
    }

    public override void WeakpointAttack(Animator animator, GameObject player, Weakpoint weakpoint, Vector3 hitLocation) {
        weakpoint.TakeDamage(baseDamage, WeaponDamageType.SPEAR, hitLocation);
        BasicAttack(animator, player);
    }
}
