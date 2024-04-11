using UnityEngine;

[CreateAssetMenu(fileName = "Soul Value", menuName = "ExodusTools/Soul Value")]
public class SoulValue : ScriptableObject {
    [SerializeField] AnimationCurve soulValueCurve;

    public float SampleValue(float value) {
        Debug.Log(soulValueCurve.Evaluate(value));
        return soulValueCurve.Evaluate(value);
    }
}
