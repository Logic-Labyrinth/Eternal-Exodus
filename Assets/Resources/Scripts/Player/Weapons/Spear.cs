using System.Collections.Generic;
using LexUtils.Events;
using TEE.Environment;
using TEE.Health;
using TEE.Player.Camera;
using TEE.Player.Movement;
using UnityEngine;

namespace TEE.Player.Weapons {
    [CreateAssetMenu(fileName = "Spear", menuName = "ExodusTools/Weapon/Spear")]
    public class Spear : Weapon {
        PlayerDashing    playerDash;
        LayerMask        enemyLayer = -1;
        List<GameObject> spearTargets;

        static readonly int AnimatorTriggerSpearAttack  = Animator.StringToHash("SpearAttack");
        static readonly int AnimatorTriggerSpearSpecial = Animator.StringToHash("SpearSpecial");

        public override void BasicAttack(Animator animator) {
            if (enemyLayer < 0) enemyLayer = LayerMask.NameToLayer("Enemy");
            animator.SetTrigger(AnimatorTriggerSpearAttack);
            PlayBasicAttackSound();
            bool hasEnemy = false;

            spearTargets = CustomCapsuleCollider.GetAllObjects(
                UnityEngine.Camera.main.transform,
                UnityEngine.Camera.main.transform.forward,
                1,
                attackRange
            );

            foreach (var target in spearTargets) {
                if (target.layer == enemyLayer) {
                    target.GetComponent<HealthSystem>()?.TakeDamage(baseDamage, WeaponDamageType.Spear);
                    hasEnemy = true;
                }

                if (target.CompareTag("Breakable")) target.GetComponent<BreakableObject>().Break();
            }

            if (hasEnemy) {
                EventForge.Float.Get("Player.Trauma").Invoke(0.2f);
                FrameHang.Instance.ExecFrameHang(basicFreezeFrame, 0.05f);
            } else
                EventForge.Float.Get("Player.Trauma").Invoke(0.1f);
        }

        public override void SpecialAttack(Animator animator, GameObject player) {
            if (!playerDash) playerDash = player.GetComponent<PlayerDashing>();

            animator.SetTrigger(AnimatorTriggerSpearSpecial);
            playerDash.Dash();
            PlaySpecialAttackSound();
            EventForge.Float.Get("Player.Trauma").Invoke(0.2f);
        }

        public override void WeakpointAttack(Animator animator, Weakpoint weakpoint) {
            weakpoint.TakeDamage(baseDamage, WeaponDamageType.Spear);
            BasicAttack(animator);
        }
    }
}