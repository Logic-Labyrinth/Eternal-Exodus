using UnityEngine;

[CreateAssetMenu(fileName = "Soul Value", menuName = "ExodusTools/Soul Value")]
public class SoulValue : ScriptableObject {
    [SerializeField] int valueCap = 50;
    [SerializeField] int soulValue = 1;
    [SerializeField] EnemyType soulType;
    [SerializeField] AnimationCurve soulValueCurve;
    int souls = 0;

    public float GetSoulValue() {
        // Debug.Log(soulValueCurve.Evaluate(value));
        Debug.LogError(soulValueCurve.Evaluate(souls / valueCap) * valueCap);
        return soulValueCurve.Evaluate(souls / valueCap) * valueCap;
    }

    public float GetSoulValue(int count) {
        // Debug.Log(soulValueCurve.Evaluate(value));
        int value = count * soulValue > valueCap ? valueCap : count * soulValue;
        // Debug.Log(soulType + ": " + value);
        Debug.Log("Count: " + count + ", Value: " + value + "Eval: " + soulValueCurve.Evaluate(value * 1.0f / valueCap ) * valueCap);
        Debug.Log("Evaluation: " + soulValueCurve.Evaluate(value * 1.0f / valueCap) * valueCap);
        return soulValueCurve.Evaluate(value * 1.0f / valueCap) * valueCap;
    }

    public int GetSoulCount() {
        return souls;
    }

    public void ConsumeSoul() {
        if (souls >= valueCap) return;
        souls += soulValue;
        Debug.LogError("Consumed Souls Value: " + souls);
    }
}
