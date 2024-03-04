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
    var currentWeapon = weaponObjects[activeWeaponIndex];
    if (Input.GetAxis("Cycle Weapons") > 0 || Input.GetButtonDown("Cycle Next Weapon")) CycleToNextWeapon();
    if (Input.GetAxis("Cycle Weapons") < 0 || Input.GetButtonDown("Cycle Prev Weapon")) CycleToPreviousWeapon();

    if (Input.GetButtonDown("Select Weapon 1")) SetActiveWeapon(0);
    if (Input.GetButtonDown("Select Weapon 2")) SetActiveWeapon(1);
    if (Input.GetButtonDown("Select Weapon 3")) SetActiveWeapon(2);

    if (Input.GetButtonDown("Basic Attack") || GetTriggerDown(false)) {
      if (Physics.Raycast(cameraReference.transform.position, cameraReference.transform.forward, out RaycastHit hit, currentWeapon.weapon.attackRange)) {
        if (!hit.collider.CompareTag("Enemy")) return;

        Instantiate(hitVFXPrefab, hit.point, Quaternion.identity).GetComponent<HitVFX>().Play();
        currentWeapon.weapon.BasicAttack(playerReference, hit.collider.transform.parent.GetComponent<HealthSystem>());
      }
    }

    if (Input.GetButtonDown("Special Attack") || GetTriggerDown(true)) {
      if (!currentWeapon.canUseSpecialAttack) return;
      currentWeapon.PutOnCD();
      currentWeapon.weapon.SpecialAttack(playerReference, null);
      StartCoroutine(ResetSpecialAbility(activeWeaponIndex));
    }

    if (Input.GetButtonUp("Special Attack") || GetTriggerUp(true)) {
      currentWeapon.weapon.SpecialRelease(playerReference, null);
    }
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

  IEnumerator ResetSpecialAbility(int weaponIndex) {
    int index = weaponIndex;
    yield return new WaitForSeconds(weaponObjects[index].cooldown);
    weaponObjects[index].PutOffCD();
  }

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
}
