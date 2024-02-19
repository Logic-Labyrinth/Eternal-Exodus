using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spear", menuName = "ExodusTools/Weapon/Spear")]
public class Spear : Weapon
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