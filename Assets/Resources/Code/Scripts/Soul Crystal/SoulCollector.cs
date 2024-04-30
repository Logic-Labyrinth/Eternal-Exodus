using System.Collections.Generic;
using UnityEngine;

public class SoulCollector : MonoBehaviour {
    [SerializeField] int soulsNeeded = 10;
    [SerializeField] SoulValue soulValuePawn;
    [SerializeField] SoulValue soulValueRook;
    [SerializeField] SoulValue soulValueKnight;
    [SerializeField] SoulValue soulValueBishop;

    Dictionary<EnemyType, int> souls = new() {
        {EnemyType.Pawn, 0},
        {EnemyType.Rook, 0},
        {EnemyType.Knight, 0},
        {EnemyType.Bishop, 0}
    };

    float DEBUG_SCORE = 0;
    float pickupSoulScore = 0;

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Soul"))
            CollectSoul(other.GetComponent<SoulVFX>());
        if (other.gameObject.CompareTag("SoulPickup"))
            CollectPickupSoul(other.GetComponent<SoulPickupVFX>());
    }

    void CollectSoul(SoulVFX soul) {
        switch (soul.soulType) {
            case EnemyType.Pawn:
                Debug.Log("PAWN");
                souls[EnemyType.Pawn]++;
                break;
            case EnemyType.Rook:
                souls[EnemyType.Rook]++;
                break;
            case EnemyType.Knight:
                souls[EnemyType.Knight]++;
                break;
            case EnemyType.Bishop:
                souls[EnemyType.Bishop]++;
                break;
            default:
                Debug.LogError("Invalid soul type");
                break;
        }

        Destroy(soul.gameObject);
        DEBUG_SCORE = GetScore();
        if (DEBUG_SCORE >= soulsNeeded) Done();
    }

    void CollectPickupSoul(SoulPickupVFX soul) {
        pickupSoulScore += soul.soulValue;

        Destroy(soul.gameObject);
        DEBUG_SCORE = GetScore();
        if (DEBUG_SCORE >= soulsNeeded) Done();
    }

    float GetScore() {
        return soulValuePawn.GetSoulValue(souls[EnemyType.Pawn]) +
        soulValueRook.GetSoulValue(souls[EnemyType.Rook]) +
        soulValueKnight.GetSoulValue(souls[EnemyType.Knight]) +
        soulValueBishop.GetSoulValue(souls[EnemyType.Bishop]) +
        pickupSoulScore;
    }

    void Done() {
        Debug.Log("Done!");
        GameManager.Instance.EndLevel();
    }
}
