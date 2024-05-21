using System.Collections.Generic;
using UnityEngine;

public class SoulCollector : MonoBehaviour {
    [SerializeField] int soulsNeeded = 10;
    [SerializeField] SoulCrystalIcon icon;
    [SerializeField] SoulValue soulValuePawn;
    [SerializeField] SoulValue soulValueRook;
    [SerializeField] SoulValue soulValueKnight;
    [SerializeField] SoulValue soulValueBishop;

    Dictionary<EnemyType, int> souls = new() {
        {EnemyType.Pawn, 0},
        {EnemyType.Rook, 0},
        // {EnemyType.Knight, 0},
        {EnemyType.Bishop, 0}
    };

    float DEBUG_SCORE = 0;

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Soul"))
            CollectSoul(other.GetComponent<SoulVFX>());
        if (other.gameObject.CompareTag("SoulPickup"))
            CollectPickupSoul(other.GetComponent<SoulPickupVFX>());
    }

    void CollectSoul(SoulVFX soul) {
        switch (soul.soulType) {
            case EnemyType.Pawn:
                souls[EnemyType.Pawn]++;
                soulValuePawn.ConsumeSoul();
                break;
            case EnemyType.Rook:
                souls[EnemyType.Rook]++;
                soulValueRook.ConsumeSoul();
                break;
            // case EnemyType.Knight:
            //     souls[EnemyType.Knight]++;
            //     soulValueKnight.ConsumeSoul();
            //     break;
            case EnemyType.Bishop:
                souls[EnemyType.Bishop]++;
                soulValueBishop.ConsumeSoul();
                break;
            default:
                Debug.LogError("Invalid soul type");
                break;
        }

        Destroy(soul.gameObject);
        DEBUG_SCORE = GetScore();
        icon.SetProgress(DEBUG_SCORE / soulsNeeded);
        if (DEBUG_SCORE >= soulsNeeded) Done();
    }

    void CollectPickupSoul(SoulPickupVFX soul) {
        Destroy(soul.gameObject);
        DEBUG_SCORE = GetScore();
        icon.SetProgress(DEBUG_SCORE / soulsNeeded);
        if (DEBUG_SCORE >= soulsNeeded) Done();
    }

    float GetScore() {
        var score = soulValuePawn.GetSoulValue(souls[EnemyType.Pawn]) +
        soulValueRook.GetSoulValue(souls[EnemyType.Rook]) +
        // soulValueKnight.GetSoulValue(souls[EnemyType.Knight]) +
        soulValueBishop.GetSoulValue(souls[EnemyType.Bishop]);

        return score;
    }

    void Done() {
        GameManager.Instance.EndLevel();
    }
}
