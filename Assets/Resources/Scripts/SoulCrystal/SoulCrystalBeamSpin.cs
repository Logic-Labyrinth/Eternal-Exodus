using UnityEngine;

namespace TEE.SoulCrystal {
    public class SoulCrystalBeamSpin : MonoBehaviour {
        [SerializeField] float rotationSpeed = 10f;

        void Update() {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }
}