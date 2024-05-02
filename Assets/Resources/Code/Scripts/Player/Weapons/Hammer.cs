using UnityEngine;

[CreateAssetMenu(fileName = "Hammer", menuName = "ExodusTools/Weapon/Hammer")]
public class Hammer : Weapon {
    private HammerAbility hammer;

    public override void BasicAttack(Animator animator, GameObject player) {
        animator.SetTrigger("HammerAttack");
        PlayBasicAttackSound();
    }

    public override void BasicAttack(Animator animator, GameObject player, HealthSystem healthSystem, Vector3 hitLocation) {
        Debug.Log("Weapon Attack");
        animator.SetTrigger("HammerAttack");
        healthSystem.TakeDamage(baseDamage, WeaponDamageType.HAMMER, hitLocation);
    }

    public override void SpecialAttack(Animator animator, GameObject player, HealthSystem healthSystem) {
        if (hammer == null) {
            hammer = player.GetComponent<HammerAbility>();
        }

        animator.ResetTrigger("SwapHammer");
        animator.SetTrigger("HammerCharge");
        hammer.ChargeHammer();
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
