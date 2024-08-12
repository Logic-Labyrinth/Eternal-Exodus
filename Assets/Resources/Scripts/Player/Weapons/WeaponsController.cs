using System.Collections;
using System.Collections.Generic;
using LexUtils.Events;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace TEE.Player.Weapons {
    internal class WeaponObject {
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
            foreach (var weapon in weapons) {
                GameObject weaponObject = Instantiate(weapon.weaponObject, hand.transform, true);
                weaponObject.transform.SetLocalPositionAndRotation(
                    weapon.localPosition,
                    weapon.localRotation
                );
                weaponObjects.Add(new WeaponObject(weaponObject, weapon));
            }

            weaponObjects[activeWeaponIndex].WeaponObj.SetActive(true);
            HighlightWeapon(activeWeaponIndex);
            HandleInput();
        }

        void HandleInput() {
            if (disableWeaponInput) return;
            EventForge.Signal.Get("Input.Player.PreviousWeapon").AddListener(CycleToPreviousWeapon);
            EventForge.Signal.Get("Input.Player.NextWeapon").AddListener(CycleToNextWeapon);
            EventForge.Vector2.Get("Input.Player.WeaponCycle").AddListener(input => {
                switch (input.y) {
                    case > 0:
                        CycleToNextWeapon();
                        break;
                    case < 0:
                        CycleToPreviousWeapon();
                        break;
                }
            });

            EventForge.Signal.Get("Input.Player.WeaponSelect1").AddListener(() => SetActiveWeapon(0));
            EventForge.Signal.Get("Input.Player.WeaponSelect2").AddListener(() => SetActiveWeapon(1));
            EventForge.Signal.Get("Input.Player.WeaponSelect3").AddListener(() => SetActiveWeapon(2));
            EventForge.Signal.Get("Input.Player.BasicAttack.Pressed").AddListener(BasicAttack);
            EventForge.Signal.Get("Input.Player.SpecialAttack.Pressed").AddListener(SpecialAttack);
            EventForge.Signal.Get("Input.Player.SpecialAttack.Release").AddListener(SpecialRelease);
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
                x.GetComponent<Outline>().effectColor = new Color(1, 1, 1, 1f);
                x.transform.localScale                = Vector3.one;
                x.GetComponent<Image>().color         = new Color(0, 0, 0, 1f);
            });

            weaponSelectionUI
                .transform.GetChild(index)
                .GetComponent<Outline>()
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
    }
}