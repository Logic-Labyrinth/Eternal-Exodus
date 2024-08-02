using System.Collections.Generic;
using TEE.Environment;
using TEE.Health;
using TEE.Player.Camera;
using TEE.Player.Movement;
using UnityEngine;
using UnityEngine.AI;

namespace TEE.Player.Weapons {
    [CreateAssetMenu(fileName = "Sword", menuName = "ExodusTools/Weapon/Sword")]
    public class Sword : Weapon {
        PlayerMovement   playerMovement;
        LayerMask        enemyLayer = -1;
        List<GameObject> swordTargets;

        static readonly int AnimatorTriggerSwordAttack  = Animator.StringToHash("SwordAttack");
        static readonly int AnimatorTriggerSwordSpecial = Animator.StringToHash("SwordSpecial");

        public override void BasicAttack(Animator animator) {
            if (enemyLayer < 0) enemyLayer = LayerMask.NameToLayer("Enemy");
            animator.SetTrigger(AnimatorTriggerSwordAttack);
            PlayBasicAttackSound();
            CameraPositioning.Instance.InduceStress(0.05f);

            swordTargets = CustomTriggers.ArcRaycast(UnityEngine.Camera.main.transform, 120, attackRange, 20);

            foreach (var target in swordTargets) {
                if (target.layer == enemyLayer) target.GetComponent<HealthSystem>()?.TakeDamage(baseDamage, WeaponDamageType.Sword);
                if (target.CompareTag("Breakable")) target.GetComponent<BreakableObject>().Break();
                CameraPositioning.Instance.InduceStress(0.05f);
                FrameHang.Instance.ExecFrameHang(basicFreezeFrame, 0.05f);
            }
        }

        public override void SpecialAttack(Animator animator, GameObject player) {
            if (!playerMovement) playerMovement = player.GetComponent<PlayerMovement>();

            UppercutEnemies();

            animator.SetTrigger(AnimatorTriggerSwordSpecial);
            playerMovement.SwordJump();
            PlaySpecialAttackSound();
        }

        public override void WeakpointAttack(Animator animator, Weakpoint weakpoint) {
            weakpoint.TakeDamage(baseDamage, WeaponDamageType.Sword);
            BasicAttack(animator);
        }

        void UppercutEnemies() {
            if (enemyLayer < 0) enemyLayer = LayerMask.NameToLayer("Enemy");

            swordTargets = CustomTriggers.ArcRaycast(UnityEngine.Camera.main.transform, 120, attackRange, 20);
            CameraPositioning.Instance.InduceStress(0.1f);


            foreach (GameObject target in swordTargets) {
                if (target.layer == enemyLayer) {
                    target.GetComponent<HealthSystem>()?.TakeDamage(baseDamage, WeaponDamageType.Sword);
                    if (!target.TryGetComponent(out Rigidbody _)) continue;
                    target.GetComponent<NavMeshAgent>().isStopped      = true;
                    target.GetComponent<NavMeshAgent>().updatePosition = false;
                    target.GetComponent<Rigidbody>().AddForce(Vector3.up * 20, ForceMode.Impulse);
                    CameraPositioning.Instance.InduceStress(0.2f);
                    FrameHang.Instance.ExecFrameHang(basicFreezeFrame, 0.1f, 0.3f);
                }

                if (target.CompareTag("Breakable")) target.GetComponent<BreakableObject>().Break();
            }
        }
    }
}