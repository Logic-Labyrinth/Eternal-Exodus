using UnityEngine;
using UnityEngine.VFX;

public class HitVFX : MonoBehaviour {
  [SerializeField] VisualEffect vfx;
  [SerializeField, Range(0, 10)] int lifespan = 5;
  [SerializeField] float damageSizeMultiplier = 1;

  public void Play(float damage) {//todo
    Invoke(nameof(Stop), lifespan);
    gameObject.transform.localScale = damage / damageSizeMultiplier * Vector3.one;
  }

  public void Stop() {
    vfx.Stop();
    Destroy(gameObject);
  }
}
