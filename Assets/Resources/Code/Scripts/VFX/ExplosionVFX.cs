using UnityEngine;
using UnityEngine.VFX;

public class ExplosionVFX : MonoBehaviour {
    VisualEffect explosionEffect;

    void Start() {
        explosionEffect = GetComponent<VisualEffect>();
        explosionEffect.Stop();
    }

    public void Play() {
        explosionEffect.Play();
    }

    public float GetDuration() {
        return explosionEffect.GetFloat("Lifetime");
    }
}
