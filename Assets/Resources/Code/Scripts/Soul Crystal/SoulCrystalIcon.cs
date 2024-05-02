using UnityEngine;
using UnityEngine.UI;

public class SoulCrystalIcon : MonoBehaviour {
    Material material;

    void Start() {
        material = GetComponent<Image>().material;
        material.SetFloat("_Progress", 0);
    }

    public void SetProgress(float progress) {
        material.SetFloat("_Progress", progress);
    }
}
