using UnityEngine;

public class HammerSmashVFX : MonoBehaviour {
    [SerializeField] float duration = 10f;

    private void Start() {
        Destroy(gameObject, duration);
    }
}
