using UnityEngine;
using UnityEngine.VFX;

public class SpeedLinesVFX : MonoBehaviour {
  [SerializeField, Range(0, 50)] int threshold = 5;
  VisualEffect vfx;
  public float speed;

  void Start() {
    speed = 0;
    vfx = GetComponent<VisualEffect>();
  }

  void Update() {
    if (speed > threshold) {
      vfx.Play();
    } else {
      vfx.Stop();
    }
  }

  public void SetSpeed(float s) {
    speed = s;
  }
}