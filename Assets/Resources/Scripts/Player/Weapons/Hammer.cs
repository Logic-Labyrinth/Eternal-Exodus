using System.Collections.Generic;
using TEE.Environment;
using TEE.Health;
using UnityEngine;

namespace TEE.Player.Weapons {
    [CreateAssetMenu(fileName = "Hammer", menuName = "ExodusTools/Weapon/Hammer")]
    public class Hammer : Weapon {
        private HammerAbility hammer;
        LayerMask             enemyLayer = -1;
        List<GameObject>      hammerTargets;
        static readonly int   AnimatorTriggerHammerAttack    = Animator.StringToHash("HammerAttack");
        static readonly int   AnimatorTriggerSwapHammer      = Animator.StringToHash("SwapHammer");
        static readonly int   AnimatorTriggerHammerRelease   = Animator.StringToHash("HammerRelease");
        static readonly int   AnimatorTriggerHammerCharge    = Animator.StringToHash("HammerCharge");
        static readonly int   AnimatorTriggerHammerInterrupt = Animator.StringToHash("HammerInterrupt");

        public override void BasicAttack(Animator animator) {
            if (enemyLayer < 0) enemyLayer = LayerMask.NameToLayer("Enemy");
            animator.SetTrigger(AnimatorTriggerHammerAttack);
            PlayBasicAttackSound();

            hammerTargets = CustomTriggers.ArcRaycast(UnityEngine.Camera.main.transform, 120, attackRange, 20);

            foreach (GameObject target in hammerTargets) {
                if (target.layer == enemyLayer) target.GetComponent<HealthSystem>()?.TakeDamage(baseDamage, WeaponDamageType.Hammer);
                if (target.CompareTag("Breakable")) target.GetComponent<BreakableObject>().Break();
            }
        }

        public override void SpecialAttack(Animator animator, GameObject player) {
            if (hammer == null) hammer = player.GetComponent<HammerAbility>();

            animator.ResetTrigger(AnimatorTriggerSwapHammer);
            animator.ResetTrigger(AnimatorTriggerHammerRelease);
            animator.SetTrigger(AnimatorTriggerHammerCharge);
            hammer.ChargeHammer();
            PlaySpecialAttackSound();
        }

        public override void WeakpointAttack(Animator animator, Weakpoint weakpoint) {
            weakpoint.TakeDamage(baseDamage, WeaponDamageType.Hammer);
            BasicAttack(animator);
        }

        public override void SpecialRelease(Animator animator, GameObject player) {
            if (hammer == null) hammer = player.GetComponent<HammerAbility>();

            if (!hammer.isCharged) {
                animator.SetTrigger(AnimatorTriggerHammerInterrupt);
                hammer.CancelCharge();
                return;
            }

            animator.SetTrigger(AnimatorTriggerHammerRelease);
            hammer.ActivateHammerAbility(baseDamage, attackRange, this);
        }

        public override void Reset() {
            if (hammer == null) return;
            hammer.Reset();
        }
    }
}