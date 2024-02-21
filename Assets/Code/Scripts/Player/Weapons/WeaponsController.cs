using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

struct WeaponObject {
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
}

public class WeaponsController : MonoBehaviour {
    // private Weapon activeWeapon;
    private int activeWeaponIndex;
    [SerializeField]
    private GameObject hand;
    // int currentWeaponIndex;

    [TableList(AlwaysExpanded = true)]
    public List<Weapon> weapons;
    [SerializeField]
    // private List<GameObject> weaponObjects;
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
        // activeWeapon.BasicAttack(playerReference);

        if (Input.GetButtonDown("Special Attack")) {
            Debug.Log(currentWeapon.canUseSpecialAttack);
            if (!currentWeapon.canUseSpecialAttack) return;
            // Debug.Log("Special Attack");
            // activeWeapon.canUseSpecialAttack = false;
            // activeWeapon.SpecialAttack(playerReference);
            currentWeapon.canUseSpecialAttack = false;
            Debug.Log(currentWeapon.canUseSpecialAttack);
            currentWeapon.weapon.SpecialAttack(playerReference);
            StartCoroutine(ResetSpecialAbility(currentWeapon));
        }

        if (Input.GetButtonUp("Special Attack")) {
            currentWeapon.weapon.SpecialAttack(playerReference);
            // activeWeapon.SpecialRelease(playerReference);
        }
    }

    private void SetActiveWeapon(int index) {
        var currentWeapon = weaponObjects[activeWeaponIndex];
        currentWeapon.weaponObj.SetActive(false);
        activeWeaponIndex = index;
        // activeWeapon = weapons[activeWeaponIndex];
        currentWeapon = weaponObjects[activeWeaponIndex];
        currentWeapon.weaponObj.SetActive(true);
    }

    private void CycleToNextWeapon() {
        var currentWeapon = weaponObjects[activeWeaponIndex];
        currentWeapon.weaponObj.SetActive(false);
        activeWeaponIndex = (activeWeaponIndex + 1) % weapons.Count;
        // activeWeapon = weapons[activeWeaponIndex];
        currentWeapon = weaponObjects[activeWeaponIndex];
        currentWeapon.weaponObj.SetActive(true);
    }

    private void CycleToPreviousWeapon() {
        var currentWeapon = weaponObjects[activeWeaponIndex];
        currentWeapon.weaponObj.SetActive(false);
        activeWeaponIndex = (activeWeaponIndex - 1 + weapons.Count) % weapons.Count;
        // activeWeapon = weapons[activeWeaponIndex];
        currentWeapon = weaponObjects[activeWeaponIndex];
        currentWeapon.weaponObj.SetActive(true);
    }

    IEnumerator ResetSpecialAbility(WeaponObject weaponObject) {
        Debug.Log("Waiting");
        yield return new WaitForSeconds(weaponObject.cooldown);
        Debug.Log("Something");
        // weaponObject.canUseSpecialAttack = true;
    }
}
