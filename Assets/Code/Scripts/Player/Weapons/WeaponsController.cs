using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class WeaponsController : MonoBehaviour
{
    private Weapon activeWeapon;
    [SerializeField]
    private GameObject hand;
    int currentWeaponIndex;

    [TableList(AlwaysExpanded = true)]
    public List<Weapon> weapons;
    [SerializeField]
    private List<GameObject> weaponObjects;
    [SerializeField]
    private GameObject playerReference;

    /// <summary>
    /// Instantiates weapons and sets the active weapon
    /// </summary>
    private void Start()
    {
        // Loop through each weapon to create game objects
        foreach (Weapon weapon in weapons)
        {
            // Create game object
            GameObject weaponObject = Instantiate(weapon.weaponObject);
            // Set parent
            weaponObject.transform.SetParent(hand.transform);
            // Set position
            weaponObject.transform.localPosition = weapon.localPosition;
            // Set rotation
            weaponObject.transform.localRotation = weapon.localRotation;
            // Add to weapon objects list
            weaponObjects.Add(weaponObject);
            // Set all inactive
            weaponObject.SetActive(false);
            // Set first weapon active
            if (weapon == weapons[0]) {
                activeWeapon = weapon;
                currentWeaponIndex = 0;
                weaponObject.SetActive(true);
            }
        }
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetAxis("Cycle Weapons") > 0 || Input.GetKeyDown(KeyCode.E))
        {
            CycleToNextWeapon();
        }
        if (Input.GetAxis("Cycle Weapons") < 0 || Input.GetKeyDown(KeyCode.Q))
        {
            CycleToPreviousWeapon();
        }
        if (Input.GetButtonDown("Select Weapon 1")) {
            SetActiveWeapon(0);
        }
        if (Input.GetButtonDown("Select Weapon 2")) {
            SetActiveWeapon(1);
        }
        if (Input.GetButtonDown("Select Weapon 3")) {
            SetActiveWeapon(2);
        }
        if (Input.GetButtonDown("Basic Attack")) {
            activeWeapon.BasicAttack(playerReference);
        }
        if (Input.GetButtonDown("Special Attack")) {
            activeWeapon.SpecialAttack(playerReference);
        }
        if (Input.GetButtonUp("Special Attack")) {
                activeWeapon.SpecialRelease(playerReference);
        }
    }

    private void SetActiveWeapon(int index) {
        weaponObjects[currentWeaponIndex].SetActive(false);
        currentWeaponIndex = index;
        activeWeapon = weapons[currentWeaponIndex];
        weaponObjects[currentWeaponIndex].SetActive(true);
    }

    private void CycleToNextWeapon()
    {
        weaponObjects[currentWeaponIndex].SetActive(false);
        currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Count;
        activeWeapon = weapons[currentWeaponIndex];
        weaponObjects[currentWeaponIndex].SetActive(true);
    }

    private void CycleToPreviousWeapon()
    {
        weaponObjects[currentWeaponIndex].SetActive(false);
        currentWeaponIndex = ((currentWeaponIndex - 1) + weapons.Count) % weapons.Count;
        activeWeapon = weapons[currentWeaponIndex];
        weaponObjects[currentWeaponIndex].SetActive(true);
    }
}
