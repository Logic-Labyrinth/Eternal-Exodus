using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace TEE.Player.Weapons {
    class WeaponObject {
        public readonly GameObject WeaponObj;
        public readonly Weapon     Weapon;
        public          bool       CanUseBasicAttack;
        public          bool       CanUseSpecialAttack;
        public readonly float      BasicAttackCooldown;
        public readonly float      SpecialAttackCooldown;

        public WeaponObject(GameObject weaponObj, Weapon weapon) {
            WeaponObj = weaponObj;
            WeaponObj.SetActive(false);
            Weapon                = weapon;
            CanUseBasicAttack     = true;
            CanUseSpecialAttack   = true;
            BasicAttackCooldown   = 1 / weapon.attackSpeed;
            SpecialAttackCooldown = weapon.specialAttackCooldown;
        }

        public void PutOnCD() {
            CanUseSpecialAttack = false;
        }

        public void PutOffCd() {
            CanUseSpecialAttack = true;
        }
    }

    public class WeaponsController : MonoBehaviour {
        int  activeWeaponIndex;
        bool disableWeaponInput;

        [SerializeField]                          GameObject         hand;
        [SerializeField]                          Animator           animator;
        [SerializeField]                          GameObject         weaponSelectionUI;
        [TableList(AlwaysExpanded = true)] public List<Weapon>       weapons;
        [SerializeField]                          List<WeaponObject> weaponObjects;
        [SerializeField]                          GameObject         playerReference;

        static readonly int AnimatorIntegerActiveWeapon = Animator.StringToHash("Active Weapon");

        void Start() {
            activeWeaponIndex = 0;
            weaponObjects     = new List<WeaponObject>();
            for (int i = 0; i < weapons.Count; i++) {
                var        weapon       = weapons[i];
                GameObject weaponObject = Instantiate(weapon.weaponObject);
                weaponObject.transform.SetParent(hand.transform);
                weaponObject.transform.SetLocalPositionAndRotation(
                    weapon.localPosition,
                    weapon.localRotation
                );
                weaponObjects.Add(new WeaponObject(weaponObject, weapon));
            }

            weaponObjects[activeWeaponIndex].WeaponObj.SetActive(true);
            HighlightWeapon(activeWeaponIndex);
        }

        void Update() {
            HandleInput();
        }

        void HandleInput() {
            if (disableWeaponInput) return;
            if (Input.GetAxis("Cycle Weapons") > 0 || Input.GetButtonDown("Cycle Next Weapon")) CycleToNextWeapon();
            if (Input.GetAxis("Cycle Weapons") < 0 || Input.GetButtonDown("Cycle Prev Weapon")) CycleToPreviousWeapon();

            if (Input.GetButtonDown("Select Weapon 1")) SetActiveWeapon(2);
            if (Input.GetButtonDown("Select Weapon 2")) SetActiveWeapon(1);
            if (Input.GetButtonDown("Select Weapon 3")) SetActiveWeapon(0);

            if (Input.GetButtonDown("Basic Attack")   || GetTriggerDown(false)) BasicAttack();
            if (Input.GetButtonDown("Special Attack") || GetTriggerDown(true)) SpecialAttack();
            if (Input.GetButtonUp("Special Attack")   || GetTriggerUp(true)) SpecialRelease();
        }

        void SetActiveWeapon(int index) {
            var currentWeapon = weaponObjects[activeWeaponIndex];
            currentWeapon.WeaponObj.SetActive(false);
            currentWeapon.Weapon.Reset();
            activeWeaponIndex = index;
            currentWeapon     = weaponObjects[activeWeaponIndex];
            currentWeapon.WeaponObj.SetActive(true);

            animator.SetTrigger(currentWeapon.Weapon.swapAnimation);
            animator.SetInteger(AnimatorIntegerActiveWeapon, activeWeaponIndex);
            HighlightWeapon(activeWeaponIndex);
        }

        void CycleToNextWeapon() {
            SetActiveWeapon((activeWeaponIndex + 1) % weapons.Count);
        }

        void CycleToPreviousWeapon() {
            SetActiveWeapon((activeWeaponIndex - 1 + weapons.Count) % weapons.Count);
        }

        void HighlightWeapon(int index) {
            var children = weaponSelectionUI.transform.GetChildren(true);

            children.ForEach(x => {
                x.GetComponent<UnityEngine.UI.Outline>().effectColor = new Color(1, 1, 1, 1f);
                x.transform.localScale                               = Vector3.one;
                x.GetComponent<Image>().color                        = new Color(0, 0, 0, 1f);
            });

            weaponSelectionUI
                .transform.GetChild(index)
                .GetComponent<UnityEngine.UI.Outline>()
                .effectColor = new Color(0, 0, 0, 1f);
            weaponSelectionUI.transform.GetChild(index).GetComponent<Image>().color = new Color(1, 1, 1, 1f);
            weaponSelectionUI.transform.GetChild(index).transform.localScale        = new Vector3(1.3f, 1.3f, 1.3f);
        }

        void BasicAttack() {
            var currentWeapon = weaponObjects[activeWeaponIndex];
            if (!currentWeapon.CanUseBasicAttack) return;

            currentWeapon.Weapon.BasicAttack(animator);

            currentWeapon.CanUseBasicAttack = false;
            StartCoroutine(ResetBasicAttack(activeWeaponIndex));
        }

        void SpecialAttack() {
            var currentWeapon = weaponObjects[activeWeaponIndex];
            if (!currentWeapon.CanUseSpecialAttack) return;
            currentWeapon.PutOnCD();

            var col              = currentWeapon.WeaponObj.GetComponent<Collider>();
            if (col) col.enabled = true;

            currentWeapon.Weapon.SpecialAttack(animator, playerReference);
            StartCoroutine(ResetSpecialAbility(activeWeaponIndex));
        }

        void SpecialRelease() {
            var currentWeapon = weaponObjects[activeWeaponIndex];
            currentWeapon.Weapon.SpecialRelease(animator, playerReference);
        }

        IEnumerator ResetSpecialAbility(int weaponIndex) {
            yield return new WaitForSeconds(weaponObjects[weaponIndex].SpecialAttackCooldown);
            weaponObjects[weaponIndex].PutOffCd();
        }

        IEnumerator ResetBasicAttack(int weaponIndex) {
            yield return new WaitForSeconds(weaponObjects[weaponIndex].BasicAttackCooldown);
            weaponObjects[weaponIndex].CanUseBasicAttack = true;
        }

        public void DisableWeaponsInput() {
            disableWeaponInput = true;
        }

        public void EnableWeaponsInput() {
            disableWeaponInput = true;
        }

        #region Controller Input

        bool isLeftTriggerDown;
        bool isRightTriggerDown;

        bool GetTriggerDown(bool left) {
            if (left) {
                float value = Input.GetAxisRaw("Special Attack Controller");
                if (isLeftTriggerDown || !(value > 0)) return false;
                isLeftTriggerDown = true;
                return true;
            } else {
                float value = Input.GetAxisRaw("Basic Attack Controller");
                if (isRightTriggerDown || !(value > 0)) return false;
                isRightTriggerDown = true;
                return true;
            }
        }

        bool GetTriggerUp(bool left) {
            if (left) {
                float value = Input.GetAxisRaw("Special Attack Controller");
                if (!isLeftTriggerDown || value != 0) return false;
                isLeftTriggerDown = false;
                return true;
            } else {
                float value = Input.GetAxisRaw("Basic Attack Controller");
                if (!isRightTriggerDown || value != 0) return false;
                isRightTriggerDown = false;
                return true;
            }
        }

        #endregion
    }
}