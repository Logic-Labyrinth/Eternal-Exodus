using UnityEngine;

[CreateAssetMenu(fileName = "Hammer", menuName = "ExodusTools/Weapon/Hammer")]
public class Hammer : Weapon {
    private HammerAbility hammer;
    private bool isCharging = false;
    private bool isCharged = false;

    public override void BasicAttack(GameObject player) {
        if (hammer == null) {
            hammer = player.GetComponent<HammerAbility>();
        }
    }

    public override void SpecialAttack(GameObject player) {
        if (hammer == null) {
            hammer = player.GetComponent<HammerAbility>();
        }

        hammer.ChargeHammer();
    }

    public override void SpecialRelease(GameObject player) {
        if (hammer == null) {
            hammer = player.GetComponent<HammerAbility>();
        }

        hammer.ActivateHammerAbility();
    }
}
