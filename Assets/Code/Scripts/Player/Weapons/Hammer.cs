using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Hammer", menuName = "ExodusTools/Weapon/Hammer")]
public class Hammer : Weapon
{
    [SerializeField]
    private Movement playerMovement;

    public override void BasicAttack() {
        // Basic attack logic
    }

    public override void SpecialAttack() {
        // Special attack logic
    }
}
