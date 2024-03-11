using UnityEngine;

[CreateAssetMenu(fileName = "Hammer", menuName = "ExodusTools/Weapon/Hammer")]
public class Hammer : Weapon {
    private HammerAbility hammer;
    private bool isCharging = false;
    private bool isCharged = false;

    public override void BasicAttack(GameObject player, HealthSystem healthSystem, Vector3 hitLocation) {
        healthSystem.TakeDamage(baseDamage, WeaponDamageType.HAMMER, hitLocation);
    }

    public override void SpecialAttack(GameObject player, HealthSystem healthSystem) {
        if (hammer == null) {
            hammer = player.GetComponent<HammerAbility>();
        }

        hammer.ChargeHammer();
    }

    public override void SpecialRelease(GameObject player, HealthSystem healthSystem) {
        if (hammer == null) {
            hammer = player.GetComponent<HammerAbility>();
        }

        hammer.ActivateHammerAbility();
    }

    public override void Reset() {
        if(hammer == null) return;
        hammer.Reset();
    }
}
