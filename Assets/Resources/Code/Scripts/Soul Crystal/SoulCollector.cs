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

    public float score;

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Soul"))
            CollectSoul(other.GetComponent<SoulVFX>());
    }

    void CollectSoul(SoulVFX soul) {
        switch (soul.soulType) {
            case EnemyType.Pawn:
                // soulValuePawn.ConsumeSoul();
                Debug.Log("PAWN");
                souls[EnemyType.Pawn]++;
                // Debug.LogError("Consumed Pawn: " + soulValuePawn.GetSoulValue());
                break;
            case EnemyType.Rook:
                souls[EnemyType.Rook]++;
                // soulValueRook.ConsumeSoul();
                // Debug.LogError("Consumed Rook: " + soulValueRook.GetSoulValue());
                break;
            case EnemyType.Knight:
                souls[EnemyType.Knight]++;
                // soulValueKnight.ConsumeSoul();
                // Debug.LogError("Consumed Knight: " + soulValueKnight.GetSoulValue());
                break;
            case EnemyType.Bishop:
                souls[EnemyType.Bishop]++;
                // soulValueBishop.ConsumeSoul();
                // Debug.LogError("Consumed Bishop: " + soulValueBishop.GetSoulValue());
                break;
            default:
                Debug.LogError("Invalid soul type");
                break;
        }

        Destroy(soul.gameObject);
        score = GetScore();
        if (GetScore() >= soulsNeeded) Done();
    }

    float GetScore() {
        return soulValuePawn.GetSoulValue(souls[EnemyType.Pawn]) +
        soulValueRook.GetSoulValue(souls[EnemyType.Rook]) +
        soulValueKnight.GetSoulValue(souls[EnemyType.Knight]) +
        soulValueBishop.GetSoulValue(souls[EnemyType.Bishop]);
    }

    void Done() {
        Debug.Log("Done!");
        GameManager.Instance.EndLevel();
    }
}
