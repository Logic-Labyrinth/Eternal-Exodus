using UnityEngine;
using UnityEngine.VFX;

public class SoulVFX : MonoBehaviour {
    [SerializeField] Transform vfxTransform;
    [SerializeField] float speed = 0.1f;
    public int soulCount = 1;
    Vector3 target, origin;
    float timer = 0;

    void Start() {
        target = GameObject.Find("Soul Collector").transform.position;
        origin = transform.position;
    }

    void FixedUpdate() {
        transform.position = Vector3.Slerp(origin, target, timer * speed);
        timer += Time.fixedDeltaTime;
    }

    public void Consume() {
        vfxTransform.GetComponent<VisualEffect>().Stop();
    }
}
