using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class WeaponsController : MonoBehaviour
{
    private Weapon activeWeapon;
    int currentWeaponIndex;

    [TableList(AlwaysExpanded = true)]
    public List<Weapon> weapons;
    private List<GameObject> weaponObjects;

    /// <summary>
    /// Instantiates weapons and sets the active weapon
    /// </summary>
    private void Start()
    {
        // Loop through each weapon
        foreach (Weapon weapon in weapons)
        {
            // Instantiate the weapon object as a child of the current transform
            GameObject instance = Instantiate(weapon.weaponObject, transform);
            weaponObjects.Add(instance);
            // Deactivate the weapon object
            weapon.weaponObject.SetActive(false);
            // If no active weapon is set, set the current weapon as the active weapon
            if (activeWeapon == null)
            {
                activeWeapon = weapon;
                // Activate the weapon object
                weapon.weaponObject.SetActive(true);
            }
        }
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetAxis("Cycle Weapons") > 0)
        {
            CycleToNextWeapon();
        }
        if (Input.GetAxis("Cycle Weapons") < 0)
        {
            CycleToPreviousWeapon();
        }
    }

    private void CycleToNextWeapon()
    {
        currentWeaponIndex = weapons.IndexOf(activeWeapon);
        currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Count;
        activeWeapon = weapons[currentWeaponIndex];
    }

    private void CycleToPreviousWeapon()
    {
        currentWeaponIndex = weapons.IndexOf(activeWeapon);
        currentWeaponIndex = ((currentWeaponIndex - 1) + weapons.Count) % weapons.Count;
        activeWeapon = weapons[currentWeaponIndex];
    }
}
