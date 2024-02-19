using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sword", menuName = "ExodusTools/Weapon/Sword")]
public class Sword : Weapon
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
