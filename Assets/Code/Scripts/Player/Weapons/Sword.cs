using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Sword", menuName = "ExodusTools/Weapon/Sword")]
public class Sword : Weapon
{
    private PlayerMovement playerMovement;

    public override void BasicAttack(GameObject player) {
        // Basic attack logic
    }

    public override void SpecialAttack(GameObject player) {
        if (playerMovement == null) {
            playerMovement = player.GetComponent<PlayerMovement>();
        }

        playerMovement.Jump();
    }
}
