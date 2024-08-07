using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TEE.Enemy;
using TEE.Environment;
using TEE.Health;
using TEE.Player;
using TEE.VFX;
using UnityEngine;
using UnityEngine.VFX;

namespace TEE.SoulCrystal {
    [RequireComponent(typeof(CapsuleCollider))]
    public class SoulCollector : MonoBehaviour {
        [SerializeField] int             scoreNeeded = 10;
        [SerializeField] SoulCrystalIcon icon;
        [SerializeField] ExplosionVFX    explosionVFX;
        [SerializeField] float           gracePeriodSeconds = 10f;
        [SerializeField] SoulValue       soulValuePawn;
        [SerializeField] SoulValue       soulValueRook;
        [SerializeField] SoulValue       soulValueKnight;
        [SerializeField] SoulValue       soulValueBishop;
        [SerializeField] MeshRenderer    crystalMesh;
        [SerializeField] GameObject      beam;
        [SerializeField] VisualEffect    orbVFX;
        [SerializeField] Light           crystalLight;

        readonly Dictionary<EnemyType, int> souls = new() {
            { EnemyType.Pawn, 0 },
            { EnemyType.Rook, 0 },
            { EnemyType.Bishop, 0 }
        };

        float pickupSouls;
        float score;
        bool  fullyCharged;

        static readonly int ShaderPropertyHitFlashBool       = Shader.PropertyToID("_HitFlashBool");
        static readonly int ShaderPropertyEmissionMultiplier = Shader.PropertyToID("_emissionMultiplier");

        void Start() {
            Reset();
        }

        void Update() {
            if (!(score >= scoreNeeded)) return;
            fullyCharged = true;
            icon.SetBlink(true);
            StartCoroutine(ChargeCrystal());
        }

        void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag("Soul")) CollectSoul(other.GetComponent<SoulVFX>());
            if (other.gameObject.CompareTag("SoulPickup")) CollectPickupSoul(other.GetComponent<SoulPickupVFX>());
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
            score = GetScore();
            crystalMesh.materials[0].SetFloat(ShaderPropertyEmissionMultiplier, score / scoreNeeded * 0.5f);
            crystalMesh.materials[1].SetFloat(ShaderPropertyEmissionMultiplier, score / scoreNeeded * 0.5f);
            orbVFX.SetFloat("sphereScaleF", score                                                   / scoreNeeded);
        }

        void CollectPickupSoul(SoulPickupVFX soul) {
            if (fullyCharged) {
                Destroy(soul.gameObject);
                return;
            }

            pickupSouls += soul.soulValue;
            Destroy(soul.gameObject);
            score = GetScore();
        }

        float GetScore() {
            return soulValuePawn.GetSoulValue(souls[EnemyType.Pawn])     +
                   soulValueRook.GetSoulValue(souls[EnemyType.Rook])     +
                   soulValueBishop.GetSoulValue(souls[EnemyType.Bishop]) +
                   pickupSouls;
        }

        void Reset() {
            pickupSouls  = 0;
            score        = 0;
            fullyCharged = false;
            beam.SetActive(false);
            icon.SetBlink(false);
            UITimer.Instance.ResetTime();
            crystalMesh.materials[0].SetFloat(ShaderPropertyEmissionMultiplier, 0);
            crystalMesh.materials[1].SetFloat(ShaderPropertyEmissionMultiplier, 0);
            beam.transform.localScale = Vector3.zero;
            orbVFX.SetFloat("sphereScaleF", 0);

            souls[EnemyType.Pawn]   = 0;
            souls[EnemyType.Rook]   = 0;
            souls[EnemyType.Bishop] = 0;
        }

        public void Explode() {
            Debug.Log("Explode");
            if (!fullyCharged) return;
            Debug.Log("FullyCharged");
            explosionVFX.Play();
            SpawnManager.Instance.SetSpawnerActive(false);
            CrystalFlash();
            FindObjectsByType<HealthSystem>(FindObjectsSortMode.None).ToList().ForEach(x => x.KillWithoutSoul());

            StartCoroutine(RestartSpawner());
            Reset();
        }

        IEnumerator RestartSpawner() {
            yield return new WaitForSeconds(gracePeriodSeconds);
            SpawnManager.Instance.SetSpawnerActive(true);
        }

        void CrystalFlash() {
            crystalLight.intensity = 100.0f;
            crystalMesh.materials[0].SetInt(ShaderPropertyHitFlashBool, 1);
            crystalMesh.materials[1].SetInt(ShaderPropertyHitFlashBool, 1);
            StartCoroutine(CrystalFlashReset());
        }

        IEnumerator CrystalFlashReset() {
            yield return new WaitForSeconds(2f);
            crystalLight.intensity = 30.0f;
            crystalMesh.materials[0].SetInt(ShaderPropertyHitFlashBool, 0);
            crystalMesh.materials[1].SetInt(ShaderPropertyHitFlashBool, 0);
        }

        IEnumerator ChargeCrystal() {
            beam.SetActive(true);
            float timer = 0;

            while (timer < 1) {
                timer                     += Time.deltaTime;
                beam.transform.localScale =  Vector3.Lerp(Vector3.zero, Vector3.one, timer);
                yield return null;
            }
        }
    }
}