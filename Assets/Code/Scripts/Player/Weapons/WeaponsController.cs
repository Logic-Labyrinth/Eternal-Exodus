using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

class WeaponObject {
    public GameObject weaponObj;
    public Weapon weapon;
    public bool canUseSpecialAttack;
    public float cooldown;

    public WeaponObject(GameObject weaponObj, Weapon weapon) {
        this.weaponObj = weaponObj;
        this.weaponObj.SetActive(false);
        this.weapon = weapon;
        canUseSpecialAttack = true;
        cooldown = weapon.specialAttackCooldown;
    }

    public void PutOnCD() {
        canUseSpecialAttack = false;
    }

    public void PutOffCD() {
        canUseSpecialAttack = true;
    }
}

public class WeaponsController : MonoBehaviour {
    private int activeWeaponIndex;
    [SerializeField]
    private GameObject hand;

    [TableList(AlwaysExpanded = true)]
    public List<Weapon> weapons;
    [SerializeField]
    private List<WeaponObject> weaponObjects;
    [SerializeField]
    private GameObject playerReference;

    /// <summary>
    /// Instantiates weapons and sets the active weapon
    /// </summary>
    private void Start() {
        activeWeaponIndex = 0;
        weaponObjects = new List<WeaponObject>();
        // Loop through each weapon to create game objects
        for (int i = 0; i < weapons.Count; i++) {
            var weapon = weapons[i];
            // Create game object
            GameObject weaponObject = Instantiate(weapon.weaponObject);
            // Set parent
            weaponObject.transform.SetParent(hand.transform);
            // Set position & rotation
            weaponObject.transform.SetLocalPositionAndRotation(weapon.localPosition, weapon.localRotation);
            // Add to weapon objects list
            weaponObjects.Add(new WeaponObject(weaponObject, weapon));
        }

        weaponObjects[activeWeaponIndex].weaponObj.SetActive(true);
    }

    private void Update() {
        HandleInput();
    }

    private void HandleInput() {
        var currentWeapon = weaponObjects[activeWeaponIndex];
        if (Input.GetAxis("Cycle Weapons") > 0 || Input.GetKeyDown(KeyCode.E)) CycleToNextWeapon();
        if (Input.GetAxis("Cycle Weapons") < 0 || Input.GetKeyDown(KeyCode.Q)) CycleToPreviousWeapon();

        if (Input.GetButtonDown("Select Weapon 1")) SetActiveWeapon(0);
        if (Input.GetButtonDown("Select Weapon 2")) SetActiveWeapon(1);
        if (Input.GetButtonDown("Select Weapon 3")) SetActiveWeapon(2);

        if (Input.GetButtonDown("Basic Attack"))
            currentWeapon.weapon.BasicAttack(playerReference);

        if (Input.GetButtonDown("Special Attack")) {
            if (!weaponObjects[activeWeaponIndex].canUseSpecialAttack) return;
            weaponObjects[activeWeaponIndex].PutOnCD();
            weaponObjects[activeWeaponIndex].weapon.SpecialAttack(playerReference);
            StartCoroutine(ResetSpecialAbility(activeWeaponIndex));
        }

        if (Input.GetButtonUp("Special Attack")) {
            weaponObjects[activeWeaponIndex].weapon.SpecialRelease(playerReference);
        }
    }

    private void SetActiveWeapon(int index) {
        var currentWeapon = weaponObjects[activeWeaponIndex];
        currentWeapon.weaponObj.SetActive(false);
        activeWeaponIndex = index;
        currentWeapon = weaponObjects[activeWeaponIndex];
        currentWeapon.weaponObj.SetActive(true);
    }

    private void CycleToNextWeapon() {
        var currentWeapon = weaponObjects[activeWeaponIndex];
        currentWeapon.weaponObj.SetActive(false);
        activeWeaponIndex = (activeWeaponIndex + 1) % weapons.Count;
        currentWeapon = weaponObjects[activeWeaponIndex];
        currentWeapon.weaponObj.SetActive(true);
    }

    private void CycleToPreviousWeapon() {
        var currentWeapon = weaponObjects[activeWeaponIndex];
        currentWeapon.weaponObj.SetActive(false);
        activeWeaponIndex = (activeWeaponIndex - 1 + weapons.Count) % weapons.Count;
        currentWeapon = weaponObjects[activeWeaponIndex];
        currentWeapon.weaponObj.SetActive(true);
    }

    IEnumerator ResetSpecialAbility(int weaponIndex) {
        int index = weaponIndex;
        yield return new WaitForSeconds(weaponObjects[index].cooldown);
        Debug.Log("Reset Special Ability");
        weaponObjects[index].PutOffCD();
    }
}
