using LexUtils.Events;
using LexUtils.Singleton;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TEE.Input {
    public class InputManager : PersistentSingleton<InputManager> {
        static        bool IsInputEnabled { get; set; } = true;
        public static void Enable()       => IsInputEnabled = true;
        public static void Disable()      => IsInputEnabled = false;

        static InputActionMap playerInputMap;
        static InputActionMap interfaceInputMap;

        protected override void Awake() {
            base.Awake();
            playerInputMap    = InputSystem.actions.FindActionMap("Player");
            interfaceInputMap = InputSystem.actions.FindActionMap("Interface");

            playerInputMap["Movement"].performed       += _ => EventForge.Vector2.Get("Input.Player.Movement").Invoke(playerInputMap["Movement"].ReadValue<Vector2>());
            playerInputMap["Movement"].canceled        += _ => EventForge.Vector2.Get("Input.Player.Movement").Invoke(playerInputMap["Movement"].ReadValue<Vector2>());
            playerInputMap["WeaponCycle"].performed    += _ => EventForge.Vector2.Get("Input.Player.WeaponCycle").Invoke(playerInputMap["WeaponCycle"].ReadValue<Vector2>());
            playerInputMap["PreviousWeapon"].performed += _ => EventForge.Signal.Get("Input.Player.PreviousWeapon").Invoke();
            playerInputMap["NextWeapon"].performed     += _ => EventForge.Signal.Get("Input.Player.NextWeapon").Invoke();
            playerInputMap["WeaponSelect1"].performed  += _ => EventForge.Signal.Get("Input.Player.WeaponSelect1").Invoke();
            playerInputMap["WeaponSelect2"].performed  += _ => EventForge.Signal.Get("Input.Player.WeaponSelect2").Invoke();
            playerInputMap["WeaponSelect3"].performed  += _ => EventForge.Signal.Get("Input.Player.WeaponSelect3").Invoke();
            playerInputMap["Jump"].performed           += _ => EventForge.Signal.Get("Input.Player.Jump.Pressed").Invoke();
            playerInputMap["Crouch"].performed         += _ => EventForge.Signal.Get("Input.Player.Crouch.Pressed").Invoke();
            playerInputMap["Crouch"].canceled          += _ => EventForge.Signal.Get("Input.Player.Crouch.Released").Invoke();
            playerInputMap["BasicAttack"].performed    += _ => EventForge.Signal.Get("Input.Player.BasicAttack.Pressed").Invoke();
            playerInputMap["SpecialAttack"].performed  += _ => EventForge.Signal.Get("Input.Player.SpecialAttack.Pressed").Invoke();
            playerInputMap["SpecialAttack"].canceled   += _ => EventForge.Signal.Get("Input.Player.SpecialAttack.Released").Invoke();

            interfaceInputMap["Close"].performed += _ => EventForge.Signal.Get("Input.UI.Escape.Pressed").Invoke();
        }

        void OnEnable() {
            playerInputMap.Enable();
            interfaceInputMap.Enable();
        }

        void OnDisable() {
            playerInputMap.Disable();
            interfaceInputMap.Disable();
        }

        public static void SetCursorEnabled(bool enabled) {
            Cursor.lockState = enabled ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible   = enabled;
        }

        public static Vector2 GetLookInput() => IsInputEnabled ? playerInputMap["Look"].ReadValue<Vector2>() : Vector2.zero;
    }
}