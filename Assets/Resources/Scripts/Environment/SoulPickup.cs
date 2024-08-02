using UnityEngine;

namespace TEE.Environment {
    public class SoulPickup : MonoBehaviour {
        [SerializeField, Range(0.01f, 2f)] float soulValue = 1f;
        [SerializeField, Range(1,     20)] int   soulCount = 1;

        void OnTriggerEnter(Collider other) {
            if (!other.CompareTag("Player")) return;
            for (int i = 0; i < soulCount; i++) {
                GameObject soul = Instantiate(Resources.Load("Level/Prefabs/VFX/SoulPickupVFX"), transform.position + Vector3.up, Quaternion.identity) as GameObject;
                soul.GetComponent<SoulPickupVFX>().soulValue = soulValue;
            }

            Destroy(gameObject);
        }
    }
}