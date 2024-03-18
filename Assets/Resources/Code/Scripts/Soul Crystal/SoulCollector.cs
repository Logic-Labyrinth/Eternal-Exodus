using UnityEngine;

public class SoulCollector : MonoBehaviour {
    [SerializeField] int soulsNeeded = 10;
    [SerializeField] int soulsCollected = 0;

    void OnTriggerEnter(Collider other) {
        if (!other.gameObject.CompareTag("Soul")) return;
        CollectSoul(other.GetComponent<SoulVFX>());
    }

    void CollectSoul(SoulVFX soul) {
        soulsCollected += soul.soulCount;

        soul.Consume();
        Debug.Log("Souls Collected: " + soulsCollected);

        if (soulsCollected >= soulsNeeded) {
            Debug.Log("Soul Crystal Complete!");
        }
    }
}
