using UnityEngine;
using UnityEngine.UI;

public class SoulCrystalIcon : MonoBehaviour {
    new Transform camera;
    Material material;

    void Start() {
        camera = Camera.main.transform;
        material = GetComponent<Image>().material;
        material.SetFloat("_Progress", 0);
    }

    void Update() {
        transform.LookAt(camera);
    }

    public void SetProgress(float progress) {
        material.SetFloat("_Progress", progress);
    }
}
