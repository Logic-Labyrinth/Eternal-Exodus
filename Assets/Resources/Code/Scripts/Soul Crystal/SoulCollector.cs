using UnityEngine;

public class SoulCollector : MonoBehaviour {
    [SerializeField] int soulsNeeded = 10;
    [SerializeField] SoulValue soulValuePawn;
    [SerializeField] SoulValue soulValueRook;
    [SerializeField] SoulValue soulValueKnight;
    [SerializeField] SoulValue soulValueBishop;

    void OnTriggerEnter(Collider other) {
        if (!other.gameObject.CompareTag("Soul")) return;
        CollectSoul(other.GetComponent<SoulVFX>());
    }

    void CollectSoul(SoulVFX soul) {
        switch (soul.soulType) {
            case SoulType.PAWN:
                soulValuePawn.ConsumeSoul();
                break;
            case SoulType.ROOK:
                soulValueRook.ConsumeSoul();
                break;
            case SoulType.KNIGHT:
                soulValueKnight.ConsumeSoul();
                break;
            case SoulType.BISHOP:
                soulValueBishop.ConsumeSoul();
                break;
            default:
                Debug.LogError("Invalid soul type");
                break;
        }

        soul.Consume();
        if(GetScore() >= soulsNeeded) Done();
    }

    float GetScore() {
        return soulValuePawn.GetSoulValue() + soulValueRook.GetSoulValue() + soulValueKnight.GetSoulValue() + soulValueBishop.GetSoulValue();
    }

    void Done() {
        Debug.Log("Done!");
        GameManager.Instance.EndLevel();
    }
}
