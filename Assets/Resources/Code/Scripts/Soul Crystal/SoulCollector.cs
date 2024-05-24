using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class SoulCollector : MonoBehaviour {
    [SerializeField] int scoreNeeded = 10;
    [SerializeField] SoulCrystalIcon icon;
    [SerializeField] ExplosionVFX explosionVFX;
    [SerializeField] float gracePeriodSeconds = 10f;
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

    float pickupSouls = 0;
    float DEBUG_SCORE = 0;
    bool fullyCharged = false;

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Soul"))
            CollectSoul(other.GetComponent<SoulVFX>());
        if (other.gameObject.CompareTag("SoulPickup"))
            CollectPickupSoul(other.GetComponent<SoulPickupVFX>());
    }

    void CollectSoul(SoulVFX soul) {
        if (fullyCharged) {
            Destroy(soul.gameObject);
            return;
        }

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
                return;
        }

        Destroy(soul.gameObject);
        DEBUG_SCORE = GetScore();
        icon.SetProgress(DEBUG_SCORE / scoreNeeded);
        if (DEBUG_SCORE >= scoreNeeded) fullyCharged = true;
    }

    void CollectPickupSoul(SoulPickupVFX soul) {
        if (fullyCharged) {
            Destroy(soul.gameObject);
            return;
        }

        pickupSouls += soul.soulValue;
        Destroy(soul.gameObject);
        DEBUG_SCORE = GetScore();
        icon.SetProgress(DEBUG_SCORE / scoreNeeded);
        if (DEBUG_SCORE >= scoreNeeded) fullyCharged = true;
    }

    float GetScore() {
        var score = soulValuePawn.GetSoulValue(souls[EnemyType.Pawn]) +
        soulValueRook.GetSoulValue(souls[EnemyType.Rook]) +
        // soulValueKnight.GetSoulValue(souls[EnemyType.Knight]) +
        soulValueBishop.GetSoulValue(souls[EnemyType.Bishop]) +
        pickupSouls;

        return score;
    }

    void Reset() {
        pickupSouls = 0;
        DEBUG_SCORE = 0;
        fullyCharged = false;
        icon.SetProgress(0);

        souls[EnemyType.Pawn] = 0;
        souls[EnemyType.Rook] = 0;
        souls[EnemyType.Bishop] = 0;
    }

    public void Explode() {
        Debug.Log("Explode");
        if (!fullyCharged) return;
        Debug.Log("FullyCharged");
        explosionVFX.Play();
        SpawnManager.spawnManager.DisableSpawner();

        FindObjectsOfType<HealthSystem>().ToList().ForEach(x => {
            // x.gameObject.SetActive(false);
            x.KillWithoutSoul();
        });

        StartCoroutine(RestartSpawner());
        Reset();
    }

    IEnumerator RestartSpawner() {
        yield return new WaitForSeconds(gracePeriodSeconds);
        SpawnManager.spawnManager.EnableSpawner();
    }

    // void Done() {
    //     GameManager.Instance.EndLevel();
    // }
}
