using UnityEngine;
using UnityEngine.UI;

public class SoulCrystalIcon : MonoBehaviour {
    Material material;

    void Awake() {
        material = GetComponent<Image>().material;
        material.SetInt("_Blink", 0);
    }

    public void SetBlink(bool blink) {
        material.SetInt("_Blink", blink ? 1 : 0);
    }
}
