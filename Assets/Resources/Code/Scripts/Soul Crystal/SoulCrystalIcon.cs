using UnityEngine;
using UnityEngine.UI;

public class SoulCrystalIcon : MonoBehaviour {
    Material material;
    Animator animator;

    void Start() {
        animator = GetComponent<Animator>();
        material = GetComponent<Image>().material;
        material.SetFloat("_Progress", 0);
    }

    public void SetProgress(float progress) {
        float p = Mathf.Clamp01(progress);
        material.SetFloat("_Progress", p);
        if (p == 1) animator.SetTrigger("Fully Charged");
    }

    public void StopAnimation() {
        animator.SetTrigger("End");
    }
}
