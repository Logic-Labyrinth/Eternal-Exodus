using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Hammer", menuName = "ExodusTools/Weapon/Hammer")]
public class Hammer : Weapon
{
    private Movement playerMovement;

    public override void BasicAttack(GameObject player) {
        // Basic attack logic
    }

    public override void SpecialAttack(GameObject player) {
        
    }
}
