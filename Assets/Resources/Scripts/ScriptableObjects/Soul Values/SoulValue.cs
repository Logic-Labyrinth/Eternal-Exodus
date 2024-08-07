using TEE;
using TEE.Enemy;
using UnityEngine;

[CreateAssetMenu(fileName = "Soul Value", menuName = "ExodusTools/Soul Value")]
public class SoulValue : ScriptableObject {
    [SerializeField] int valueCap = 50;
    [SerializeField] int soulValue = 1;
    [SerializeField] EnemyType soulType;
    [SerializeField] AnimationCurve soulValueCurve;
    int souls;

    public float GetSoulValue() {
        return soulValueCurve.Evaluate(souls / valueCap) * valueCap;
    }

    public float GetSoulValue(int count) {
        int value = count * soulValue > valueCap ? valueCap : count * soulValue;
        return soulValueCurve.Evaluate(value / (float)valueCap) * valueCap;
    }

    public int GetSoulCount() {
        return souls;
    }

    public void ConsumeSoul() {
        if (souls >= valueCap) return;
        souls += soulValue;
    }
}
