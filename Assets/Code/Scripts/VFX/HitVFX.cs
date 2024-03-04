using UnityEngine;
using UnityEngine.VFX;

public class HitVFX : MonoBehaviour {
  [SerializeField] VisualEffect vfx;
  [SerializeField, Range(0, 10)] int lifespan = 5;

  public void Play() {
    Invoke(nameof(Stop), lifespan);
  }

  public void Stop() {
    vfx.Stop();
    Destroy(gameObject);
  }
}
