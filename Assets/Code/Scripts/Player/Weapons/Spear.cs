using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Spear", menuName = "ExodusTools/Weapon/Spear")]
public class Spear : Weapon
{
    private PlayerDashing playerDash;

    public override void BasicAttack(GameObject player) {
        // Basic attack logic
    }

    public override void SpecialAttack(GameObject player) {
        if (playerDash == null) {
            playerDash = player.GetComponent<PlayerDashing>();
        }
        playerDash.Dash();
    }
}