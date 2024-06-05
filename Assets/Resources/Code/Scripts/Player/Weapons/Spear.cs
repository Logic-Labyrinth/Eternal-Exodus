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
        bool hasEnemy = false;

        spearTargets = CustomCapsuleCollider.GetAllObjects(
            Camera.main.transform,
            Camera.main.transform.forward,
            1,
            attackRange
        );

        foreach (GameObject target in spearTargets) {
            if (target.layer == enemyLayer) {
                target.GetComponent<HealthSystem>()?.TakeDamage(baseDamage, WeaponDamageType.SPEAR);
                hasEnemy = true;
            }
            if (target.CompareTag("Breakable")) target.GetComponent<BreakableObject>().Break();
        }

        if (hasEnemy) {
            CameraPositioning.Instance.InduceStress(0.2f);
            FrameHang.Instance.ExecFrameHang(basicFreezeFrame, 0.05f);
        } else
            CameraPositioning.Instance.InduceStress(0.1f);

    }

    public override void SpecialAttack(Animator animator, GameObject player) {
        if (playerDash == null) playerDash = player.GetComponent<PlayerDashing>();

        animator.SetTrigger("SpearSpecial");
        playerDash.Dash();
        PlaySpecialAttackSound();
        CameraPositioning.Instance.InduceStress(0.2f);
    }

    public override void WeakpointAttack(Animator animator, Weakpoint weakpoint) {
        weakpoint.TakeDamage(baseDamage, WeaponDamageType.SPEAR);
        BasicAttack(animator);
    }
}
