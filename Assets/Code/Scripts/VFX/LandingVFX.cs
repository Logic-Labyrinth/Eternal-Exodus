using UnityEngine;
using UnityEngine.VFX;

public class LandingVFX : MonoBehaviour {
  [SerializeField] VisualEffect vfx;
  [SerializeField, Range(0, 10)] int lifespan = 5;
  [SerializeField, Range(0, 50)] int threshold = 5;

  public void Play(float intensity) {
    if (intensity < threshold) { Stop(); return; }

    gameObject.transform.localScale = intensity / threshold * Vector3.one;
    Invoke(nameof(Stop), lifespan);
  }

  public void Stop() {
    vfx.Stop();
    Destroy(gameObject);
  }
}
