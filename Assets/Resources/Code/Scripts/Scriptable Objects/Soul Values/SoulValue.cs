using UnityEngine;

public enum SoulType {
    PAWN,
    ROOK,
    KNIGHT,
    BISHOP
}

[CreateAssetMenu(fileName = "Soul Value", menuName = "ExodusTools/Soul Value")]
public class SoulValue : ScriptableObject {
    [SerializeField] int valueCap = 50;
    [SerializeField] int soulValue = 1;
    [SerializeField] SoulType soulType;
    [SerializeField] AnimationCurve soulValueCurve;
    int souls = 0;

    public float GetSoulValue() {
        // Debug.Log(soulValueCurve.Evaluate(value));
        return soulValueCurve.Evaluate(souls/valueCap) * valueCap;
    }

    public int GetSoulCount() {
        return souls;
    }

    public void ConsumeSoul() {
        if(souls >= valueCap) return;
        souls += soulValue;
    }
}
