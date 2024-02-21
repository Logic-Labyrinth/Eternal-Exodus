using UnityEngine;
using Sirenix.OdinInspector;

public class Weapon : ScriptableObject {
    [PreviewField(70, ObjectFieldAlignment.Center)]
    [TableColumnWidth(90, Resizable = false)]
    [DisableIf("isLocked")]
    public GameObject weaponObject;

    [VerticalGroup("Weapon Stats")]
    [TableColumnWidth(70)]
    [ProgressBar(1, 100)]
    [LabelText("Damage")]
    [PropertyTooltip("Base Damage")]
    [DisableIf("isLocked")]
    public int baseDamage;

    [VerticalGroup("Weapon Stats")]
    [ProgressBar(1f, 10f)]
    [LabelText("S. Multiplier")]
    [PropertyTooltip("Speed Damage Multiplier")]
    [DisableIf("isLocked")]
    public float speedDamageMultiplier;

    [VerticalGroup("Weapon Stats")]
    [ProgressBar(0f, 10f)]
    [LabelText("A. Speed")]
    [PropertyTooltip("Attack Speed")]
    [DisableIf("isLocked")]
    public float attackSpeed;

    [VerticalGroup("Weapon Stats")]
    [ProgressBar(0f, 10f)]
    [LabelText("SA. Cooldown")]
    [PropertyTooltip("Special Attack Cooldown")]
    [DisableIf("isLocked")]
    public float specialAttackCooldown;

    [VerticalGroup("Weapon Transform")]
    [LabelText("Position")]
    public Vector3 localPosition;

    [VerticalGroup("Weapon Transform")]
    [LabelText("Rotation")]
    public Quaternion localRotation;

    private bool isLocked = false;

    [Button("", ButtonSizes.Large, Icon = SdfIconType.LockFill, Stretch = false)]
    [ShowIf("IsLocked")]
    [VerticalGroup("Lock")]
    [TableColumnWidth(40, Resizable = false)]
    private void UnlockButton() {
        isLocked = false;
    }

    [Button("", ButtonSizes.Large, Icon = SdfIconType.UnlockFill, Stretch = false)]
    [HideIf("IsLocked")]
    [VerticalGroup("Lock")]
    private void LockButton() {
        isLocked = true;
    }

    private bool IsLocked() {
        return isLocked;
    }

    public virtual void BasicAttack(GameObject player) {
        // Basic attack logic
    }

    public virtual void SpecialAttack(GameObject player) {
        // Special attack logic
    }

    public virtual void SpecialRelease(GameObject player) {
        // Special attack release logic
    }
}
