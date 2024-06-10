using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

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
    [SerializeField] MeshRenderer crystalMesh;
    [SerializeField] GameObject beam;
    [SerializeField] VisualEffect orbVFX;
    [SerializeField] Light crystalLight;

    readonly Dictionary<EnemyType, int> souls = new() {
        {EnemyType.Pawn, 0},
        {EnemyType.Rook, 0},
        {EnemyType.Bishop, 0}
    };

    float pickupSouls = 0;
    float DEBUG_SCORE = 0;
    bool fullyCharged = false;

    void Start() {
        Reset();
    }

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
        crystalMesh.material.SetFloat("_emissionMultiplier", DEBUG_SCORE / scoreNeeded * 0.5f);
        orbVFX.SetFloat("sphereScaleF", DEBUG_SCORE / scoreNeeded);
        if (DEBUG_SCORE >= scoreNeeded) {
            fullyCharged = true;
            StartCoroutine(ChargeCrystal());
        }
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
        soulValueBishop.GetSoulValue(souls[EnemyType.Bishop]) +
        pickupSouls;

        return score;
    }

    void Reset() {
        pickupSouls = 0;
        DEBUG_SCORE = 0;
        fullyCharged = false;
        beam.SetActive(false);
        icon.SetProgress(0);
        icon.StopAnimation();
        UITimer.Instance.ResetTime();
        crystalMesh.material.SetFloat("_emissionMultiplier", 0);
        beam.transform.localScale = Vector3.zero;
        orbVFX.SetFloat("sphereScaleF", 0);

        souls[EnemyType.Pawn] = 0;
        souls[EnemyType.Rook] = 0;
        souls[EnemyType.Bishop] = 0;
    }

    public void Explode() {
        Debug.Log("Explode");
        if (!fullyCharged) return;
        Debug.Log("FullyCharged");
        explosionVFX.Play();
        SpawnManager.Instance.SetSpawnerActive(false);
        CrystalFlash();
        FindObjectsOfType<HealthSystem>().ToList().ForEach(x => x.KillWithoutSoul());

        StartCoroutine(RestartSpawner());
        Reset();
    }

    IEnumerator RestartSpawner() {
        yield return new WaitForSeconds(gracePeriodSeconds);
        SpawnManager.Instance.SetSpawnerActive(true);
    }

    void CrystalFlash() {
        crystalLight.intensity = 100.0f;
        crystalMesh.material.SetInt("_HitFlashBool", 1);
        StartCoroutine(CrystalFlashReset());
    }

    IEnumerator CrystalFlashReset() {
        yield return new WaitForSeconds(2f);
        crystalLight.intensity = 30.0f;
        crystalMesh.material.SetInt("_HitFlashBool", 0);
    }

    IEnumerator ChargeCrystal() {
        beam.SetActive(true);
        float timer = 0;

        while (timer < 1) {
            timer += Time.deltaTime;

            beam.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, timer);

            yield return null;
        }
    }
}
