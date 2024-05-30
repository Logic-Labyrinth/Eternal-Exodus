using UnityEngine;

[CreateAssetMenu(fileName = "Basic Freeze Frame", menuName = "ExodusTools/Basic Freeze Frame")]
public class BasicFreezeFrame : ScriptableObject {

    [SerializeField] AnimationCurve timescaleCurve;

    public float Evaluate(float x) {

        float xValue = Mathf.Clamp01(x);
        return Mathf.Clamp01(timescaleCurve.Evaluate(xValue));

    }




}
