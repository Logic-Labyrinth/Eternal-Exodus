using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class WeaponsController : MonoBehaviour
{

    [TableList(AlwaysExpanded = true)]
    public List<Weapon> weapons;

    private void HandleInput() {

    }
}
