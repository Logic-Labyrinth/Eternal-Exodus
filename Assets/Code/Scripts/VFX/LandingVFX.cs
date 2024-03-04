using UnityEngine;
using UnityEngine.VFX;

public class LandingVFX : MonoBehaviour {
  [SerializeField] VisualEffect vfx;
  [SerializeField, Range(0, 50)] int threshold = 5;

  public void Play(float intensity) {
    Debug.Log("VFX: " + intensity);
    if (intensity < threshold) { Stop(); return; }

    gameObject.transform.localScale = intensity / threshold * Vector3.one;
    Invoke(nameof(Stop), 5f);
  }

  public void Stop() {
    vfx.Stop();
    Destroy(gameObject);
  }
}
