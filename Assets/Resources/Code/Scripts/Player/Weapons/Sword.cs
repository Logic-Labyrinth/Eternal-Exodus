using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Sword", menuName = "ExodusTools/Weapon/Sword")]
public class Sword : Weapon {
    PlayerMovement playerMovement;
    LayerMask enemyLayer = -1;
    List<GameObject> swordTargets;

    public override void BasicAttack(Animator animator) {
        if (enemyLayer < 0) enemyLayer = LayerMask.NameToLayer("Enemy");
        animator.SetTrigger("SwordAttack");
        PlayBasicAttackSound();

        swordTargets = CustomTriggers.ArcRaycast(Camera.main.transform, 120, attackRange, 20);

        foreach (GameObject target in swordTargets) {
            if (target.layer == enemyLayer) target.GetComponent<HealthSystem>().TakeDamage(baseDamage, WeaponDamageType.SWORD);
            if (target.CompareTag("Breakable")) target.GetComponent<BreakableObject>().Break();
        }
    }

    public override void SpecialAttack(Animator animator, GameObject player) {
        if (playerMovement == null) playerMovement = player.GetComponent<PlayerMovement>();

        UppercutEnemies();

        animator.SetTrigger("SwordSpecial");
        playerMovement.SwordJump();
        PlaySpecialAttackSound();
    }

    public override void WeakpointAttack(Animator animator, Weakpoint weakpoint) {
        weakpoint.TakeDamage(baseDamage, WeaponDamageType.SWORD);
        BasicAttack(animator);
    }

    void UppercutEnemies() {
        if (enemyLayer < 0) enemyLayer = LayerMask.NameToLayer("Enemy");

        swordTargets = CustomTriggers.ArcRaycast(Camera.main.transform, 120, attackRange, 20);
        CameraPositioning.Instance.ShakeCamera(shakeMagnitude, shakeDuration);
        FindObjectOfType<FrameHang>().ExecFrameHang(0.1f);

        foreach (GameObject target in swordTargets) {
            if (target.layer == enemyLayer) {
                target.GetComponent<HealthSystem>().TakeDamage(baseDamage, WeaponDamageType.SWORD);
                if (!target.TryGetComponent(out Rigidbody r)) continue;
                target.GetComponent<NavMeshAgent>().isStopped = true;
                target.GetComponent<NavMeshAgent>().updatePosition = false;
                target.GetComponent<Rigidbody>().AddForce(Vector3.up * 20, ForceMode.Impulse);
            }

            if (target.CompareTag("Breakable")) target.GetComponent<BreakableObject>().Break();
        }
    }
}
