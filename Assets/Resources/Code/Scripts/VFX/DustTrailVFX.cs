using UnityEngine;
using UnityEngine.VFX;

public class DustTrailVFX : MonoBehaviour {
  [SerializeField, Range(0, 50)] int threshold = 5;
  VisualEffect vfx;
  float speed;
  bool canPlay;

  void Start() {
    speed = 0;
    canPlay = false;
    vfx = GetComponent<VisualEffect>();
  }

  void Update() {
    if (!canPlay || speed < threshold) {
      vfx.Stop();
      return;
    }
    vfx.Play();
  }

  public void SetSpeed(float s) {
    speed = s;
  }

  public void SetCanPlay(bool b) {
    canPlay = b;
  }
}
