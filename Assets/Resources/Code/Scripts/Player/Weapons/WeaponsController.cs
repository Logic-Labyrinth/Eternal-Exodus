using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

class WeaponObject {
    public GameObject weaponObj;
    public Weapon weapon;
    public bool canUseBasicAttack;
    public bool canUseSpecialAttack;
    public float basicAttackCooldown;
    public float specialAttackCooldown;

    public WeaponObject(GameObject weaponObj, Weapon weapon) {
        this.weaponObj = weaponObj;
        this.weaponObj.SetActive(false);
        this.weapon = weapon;
        canUseBasicAttack = true;
        canUseSpecialAttack = true;
        basicAttackCooldown = 1 / weapon.attackSpeed;
        specialAttackCooldown = weapon.specialAttackCooldown;
        // foreach(Sound s in weapon.basicAttackSounds) {
        //     AudioManager.instance.RegisterNewSound(s);
        // }
    }

    public void PutOnCD() {
        canUseSpecialAttack = false;
    }

    public void PutOffCD() {
        canUseSpecialAttack = true;
    }
}

public class WeaponsController : MonoBehaviour {
    int activeWeaponIndex;
    [SerializeField] GameObject hand;

    [TableList(AlwaysExpanded = true)] public List<Weapon> weapons;
    [SerializeField] List<WeaponObject> weaponObjects;
    [SerializeField] GameObject playerReference;
    [SerializeField] Camera cameraReference;
    [SerializeField] GameObject hitVFXPrefab;

    void Start() {
        activeWeaponIndex = 0;
        weaponObjects = new List<WeaponObject>();
        for (int i = 0; i < weapons.Count; i++) {
            var weapon = weapons[i];
            GameObject weaponObject = Instantiate(weapon.weaponObject);
            weaponObject.transform.SetParent(hand.transform);
            weaponObject.transform.SetLocalPositionAndRotation(weapon.localPosition, weapon.localRotation);
            weaponObjects.Add(new WeaponObject(weaponObject, weapon));
        }

        weaponObjects[activeWeaponIndex].weaponObj.SetActive(true);
    }

    void Update() {
        HandleInput();
    }

    void HandleInput() {
        if (Input.GetAxis("Cycle Weapons") > 0 || Input.GetButtonDown("Cycle Next Weapon")) CycleToNextWeapon();
        if (Input.GetAxis("Cycle Weapons") < 0 || Input.GetButtonDown("Cycle Prev Weapon")) CycleToPreviousWeapon();

        if (Input.GetButtonDown("Select Weapon 1")) SetActiveWeapon(0);
        if (Input.GetButtonDown("Select Weapon 2")) SetActiveWeapon(1);
        if (Input.GetButtonDown("Select Weapon 3")) SetActiveWeapon(2);

        if (Input.GetButtonDown("Basic Attack") || GetTriggerDown(false)) BasicAttack();
        if (Input.GetButtonDown("Special Attack") || GetTriggerDown(true)) SpecialAttack();
        if (Input.GetButtonUp("Special Attack") || GetTriggerUp(true)) SpecialRelease();
    }

    void SetActiveWeapon(int index) {
        var currentWeapon = weaponObjects[activeWeaponIndex];
        currentWeapon.weaponObj.SetActive(false);
        activeWeaponIndex = index;
        currentWeapon = weaponObjects[activeWeaponIndex];
        currentWeapon.weaponObj.SetActive(true);
    }

    void CycleToNextWeapon() {
        var currentWeapon = weaponObjects[activeWeaponIndex];
        currentWeapon.weaponObj.SetActive(false);
        currentWeapon.weapon.Reset();
        activeWeaponIndex = (activeWeaponIndex + 1) % weapons.Count;
        currentWeapon = weaponObjects[activeWeaponIndex];
        currentWeapon.weaponObj.SetActive(true);
    }

    void CycleToPreviousWeapon() {
        var currentWeapon = weaponObjects[activeWeaponIndex];
        currentWeapon.weaponObj.SetActive(false);
        currentWeapon.weapon.Reset();
        activeWeaponIndex = (activeWeaponIndex - 1 + weapons.Count) % weapons.Count;
        currentWeapon = weaponObjects[activeWeaponIndex];
        currentWeapon.weaponObj.SetActive(true);
    }

    void BasicAttack() {
        var currentWeapon = weaponObjects[activeWeaponIndex];
        bool hit = Physics.Raycast(
            cameraReference.transform.position,
            cameraReference.transform.forward,
            out RaycastHit raycastHit,
            currentWeapon.weapon.attackRange
        );

        if (!currentWeapon.canUseBasicAttack) return;

        if (hit && raycastHit.collider.CompareTag("Enemy"))
            currentWeapon.weapon.BasicAttack(playerReference, raycastHit.collider.transform.parent.GetComponent<HealthSystem>(), raycastHit.point);
        else
            currentWeapon.weapon.BasicAttack(playerReference);

        currentWeapon.canUseBasicAttack = false;
        StartCoroutine(ResetBasicAttack(activeWeaponIndex));
    }

    void SpecialAttack() {
        var currentWeapon = weaponObjects[activeWeaponIndex];
        if (!currentWeapon.canUseSpecialAttack) return;
        currentWeapon.PutOnCD();
        currentWeapon.weapon.SpecialAttack(playerReference, null);
        StartCoroutine(ResetSpecialAbility(activeWeaponIndex));
    }

    void SpecialRelease() {
        var currentWeapon = weaponObjects[activeWeaponIndex];
        currentWeapon.weapon.SpecialRelease(playerReference, null);
    }

    IEnumerator ResetSpecialAbility(int weaponIndex) {
        int index = weaponIndex;
        yield return new WaitForSeconds(weaponObjects[index].specialAttackCooldown);
        weaponObjects[index].PutOffCD();
    }

    IEnumerator ResetBasicAttack(int weaponIndex) {
        int index = weaponIndex;
        yield return new WaitForSeconds(weaponObjects[index].basicAttackCooldown);
        weaponObjects[index].canUseBasicAttack = true;
    }

    #region Controller Input
    bool isLeftTriggerDown = false;
    bool isRightTriggerDown = false;
    bool GetTriggerDown(bool left) {
        if (left) {
            float value = Input.GetAxisRaw("Special Attack Controller");
            if (!isLeftTriggerDown && value > 0) {
                isLeftTriggerDown = true;
                return true;
            }
            return false;
        } else {
            float value = Input.GetAxisRaw("Basic Attack Controller");
            if (!isRightTriggerDown && value > 0) {
                isRightTriggerDown = true;
                return true;
            }
            return false;
        }
    }

    bool GetTriggerUp(bool left) {
        if (left) {
            float value = Input.GetAxisRaw("Special Attack Controller");
            if (isLeftTriggerDown && value == 0) {
                isLeftTriggerDown = false;
                return true;
            }
            return false;
        } else {
            float value = Input.GetAxisRaw("Basic Attack Controller");
            if (isRightTriggerDown && value == 0) {
                isRightTriggerDown = false;
                return true;
            }
            return false;
        }
    }
    #endregion
}
