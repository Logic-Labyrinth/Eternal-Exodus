using UnityEngine;
using Sirenix.OdinInspector;

public class Weapon : ScriptableObject {
    #region Weapon Stats
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
    [TableColumnWidth(70)]
    [ProgressBar(0.1, 10)]
    [LabelText("Range")]
    [PropertyTooltip("Attack Range")]
    [DisableIf("isLocked")]
    public float attackRange;

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

    [VerticalGroup("Sounds")]
    [DisableIf("isLocked")]
    public Sound[] basicAttackSounds;

    [VerticalGroup("Sounds")]
    [DisableIf("isLocked")]
    public Sound[] specialAttackSounds;

    [VerticalGroup("Animations")]
    [DisableIf("isLocked")]
    public string swapAnimation;
    #endregion

    #region Locking
    bool isLocked = false;
    [Button("", ButtonSizes.Large, Icon = SdfIconType.LockFill, Stretch = false)]
    [ShowIf("IsLocked")]
    [VerticalGroup("Lock")]
    [TableColumnWidth(40, Resizable = false)]
    void UnlockButton() { isLocked = false; }

    [Button("", ButtonSizes.Large, Icon = SdfIconType.UnlockFill, Stretch = false)]
    [HideIf("IsLocked")]
    [VerticalGroup("Lock")]
    void LockButton() { isLocked = true; }
    bool IsLocked() { return isLocked; }
    #endregion

    public virtual void BasicAttack(Animator animator, GameObject player, HealthSystem healthSystem, Vector3 hitLocation) {
        // Basic attack logic
    }

    public virtual void BasicAttack(Animator animator, GameObject player) {
        // Basic attack logic
    }

    public virtual void SpecialAttack(Animator animator, GameObject player, HealthSystem healthSystem) {
        // Special attack logic
    }

    public virtual void SpecialRelease(Animator animator, GameObject player, HealthSystem healthSystem) {
        // Special attack release logic
    }

    public virtual void Reset() {
        // Reset various variables for each weapon
    }

    protected void PlayBasicAttackSound() {
        SoundFXManager.Instance.PlayRandom(basicAttackSounds);
    }

    protected void PlaySpecialAttackSound() {
        SoundFXManager.Instance.PlayRandom(specialAttackSounds);
    }
}
