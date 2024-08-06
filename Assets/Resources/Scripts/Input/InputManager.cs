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

            playerInputMap["Movement"].performed += _ => EventForge.Vector2.Get("Input.Player.Movement").Invoke(playerInputMap["Movement"].ReadValue<Vector2>());
            playerInputMap["Movement"].canceled  += _ => EventForge.Vector2.Get("Input.Player.Movement").Invoke(playerInputMap["Movement"].ReadValue<Vector2>());
            playerInputMap["WeaponCycle"].performed  += _ => EventForge.Vector2.Get("Input.Player.WeaponCycle").Invoke(playerInputMap["WeaponCycle"].ReadValue<Vector2>());
            playerInputMap["WeaponSelect"].performed += _ => EventForge.Integer.Get("Input.Player.WeaponSelect").Invoke(playerInputMap["WeaponSelect"].ReadValue<int>());
            playerInputMap["Jump"].performed   += _ => EventForge.Signal.Get("Input.Player.Jump.Pressed").Invoke();
            playerInputMap["Crouch"].performed += _ => EventForge.Signal.Get("Input.Player.Crouch.Pressed").Invoke();
            playerInputMap["Crouch"].canceled  += _ => EventForge.Signal.Get("Input.Player.Crouch.Released").Invoke();
            playerInputMap["BasicAttack"].performed   += _ => EventForge.Signal.Get("Input.Player.BasicAttack.Pressed").Invoke();
            playerInputMap["SpecialAttack"].performed += _ => EventForge.Signal.Get("Input.Player.SpecialAttack.Pressed").Invoke();
            playerInputMap["SpecialAttack"].canceled  += _ => EventForge.Signal.Get("Input.Player.SpecialAttack.Released").Invoke();
            
            interfaceInputMap["Close"].performed += _ => EventForge.Signal.Get("Input.UI.Escape.Pressed").Invoke();
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

        // public static Vector2 GetMovementInput() => IsInputEnabled ? playerInputMap["Movement"].ReadValue<Vector2>() : Vector2.zero;
        public static Vector2 GetLookInput() => IsInputEnabled ? playerInputMap["Look"].ReadValue<Vector2>() : Vector2.zero;
    }
}