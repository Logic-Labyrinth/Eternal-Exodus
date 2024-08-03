using LexUtils.Events;
using LexUtils.Singleton;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TEE.Input {
    public class InputManager : PersistentSingleton<InputManager> {
        bool        IsInputEnabled { get; set; } = true;
        public void Enable()       => IsInputEnabled = true;
        public void Disable()      => IsInputEnabled = false;

        InputActionMap playerInputMap;

        protected override void Awake() {
            base.Awake();
            playerInputMap = InputSystem.actions.FindActionMap("Player");

            playerInputMap["Movement"].performed    += _ => EventForge.Vector2.Get("Input.Player.Movement").Invoke(playerInputMap["Movement"].ReadValue<Vector2>());
            playerInputMap["Look"].performed        += _ => EventForge.Vector2.Get("Input.Player.Look").Invoke(playerInputMap["Look"].ReadValue<Vector2>());
            playerInputMap["WeaponCycle"].performed += _ => EventForge.Vector2.Get("Input.Player.WeaponCycle").Invoke(playerInputMap["WeaponCycle"].ReadValue<Vector2>());

            playerInputMap["WeaponSelect"].performed += _ => EventForge.Integer.Get("Input.Player.WeaponSelect").Invoke(playerInputMap["WeaponSelect"].ReadValue<int>());

            playerInputMap["Jump"].performed          += _ => EventForge.Signal.Get("Input.Player.Jump").Invoke();
            playerInputMap["Crouch"].performed        += _ => EventForge.Signal.Get("Input.Player.Crouch").Invoke();
            playerInputMap["BasicAttack"].performed   += _ => EventForge.Signal.Get("Input.Player.BasicAttack").Invoke();
            playerInputMap["SpecialAttack"].performed += _ => EventForge.Signal.Get("Input.Player.SpecialAttack").Invoke();
            playerInputMap["SpecialAttack"].canceled += _ => EventForge.Signal.Get("Input.Player.SpecialAttack.Release").Invoke();
        }

        void OnEnable() {
            playerInputMap.Enable();
        }

        void OnDisable() {
            playerInputMap.Disable();
        }

        public static void SetCursorEnabled(bool enabled) {
            Cursor.lockState = enabled ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible   = enabled;
        }
    }
}