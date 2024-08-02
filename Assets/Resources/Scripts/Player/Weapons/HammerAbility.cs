using System.Collections;
using System.Collections.Generic;
using TEE.Audio;
using TEE.Environment;
using TEE.Health;
using TEE.Player.Camera;
using TEE.SoulCrystal;
using UnityEngine;
using UnityEngine.UI;

namespace TEE.Player.Weapons {
    public class HammerAbility : MonoBehaviour {
        [SerializeField] float         chargeTime = 1f;
        [SerializeField] BoxCollider   impactArea;
        [SerializeField] float         hammerForce;
        [SerializeField] float         enemyBounceMultiplier;
        [SerializeField] HammerRaycast hammerRaycast;
        [SerializeField] GameObject    hammerVFX;
        [SerializeField] Sound[]       hammerImpactSounds;
        [SerializeField] Sound[]       hammerChargeSounds;
        [SerializeField] Image         hammerChargeBar;
        [SerializeField] Color         chargeColor;

        Material         hammerChargeBarMaterial;
        LayerMask        enemyLayer, groundLayer;
        bool             isCharging;
        public bool      isCharged;
        Rigidbody        rb;
        Transform        orientation;
        Coroutine        storedCoroutine;
        float            timer;
        List<GameObject> hammerTargets;

        static readonly int ShaderPropertyColor    = Shader.PropertyToID("_Color");
        static readonly int ShaderPropertyProgress = Shader.PropertyToID("_Progress");

        void Start() {
            rb                      = GetComponent<Rigidbody>();
            orientation             = transform.Find("Orientation");
            enemyLayer              = LayerMask.NameToLayer("Enemy");
            groundLayer             = LayerMask.NameToLayer("Ground");
            hammerChargeBarMaterial = hammerChargeBar.material;
            hammerChargeBar.gameObject.SetActive(false);
        }

        IEnumerator CompleteCharge() {
            yield return new WaitForSeconds(chargeTime);
            isCharged = isCharging;
        }

        public void ChargeHammer() {
            isCharging = true;
            SoundFXManager.Instance.PlayRandom(hammerChargeSounds);
            storedCoroutine = StartCoroutine(CompleteCharge());
            timer           = 0;
            hammerChargeBarMaterial.SetColor(ShaderPropertyColor, chargeColor);
            hammerChargeBarMaterial.SetFloat(ShaderPropertyProgress, 0);
            hammerChargeBar.gameObject.SetActive(true);
        }

        public void CancelCharge() {
            StopCoroutine(storedCoroutine);
            isCharging = false;
            isCharged  = false;
            timer      = 0;
            hammerChargeBarMaterial.SetColor(ShaderPropertyColor, chargeColor);
            hammerChargeBarMaterial.SetFloat(ShaderPropertyProgress, 0);
            hammerChargeBar.gameObject.SetActive(false);
        }

        private void Update() {
            if (isCharging) {
                if (timer <= chargeTime) {
                    hammerChargeBarMaterial.SetFloat(ShaderPropertyProgress, timer / chargeTime);
                    timer += Time.deltaTime;
                } else {
                    hammerChargeBarMaterial.SetColor(ShaderPropertyColor, Color.white);
                    hammerChargeBarMaterial.SetFloat(ShaderPropertyProgress, 1);
                }
            } else {
                hammerChargeBarMaterial.SetColor(ShaderPropertyColor, chargeColor);
                hammerChargeBarMaterial.SetFloat(ShaderPropertyProgress, 0);
            }
        }

        public void ActivateHammerAbility(int damage, float range, Hammer hammer) {
            if (isCharged) {
                bool hasEnemy = false, hasGround = false, hasCrystal = false;
                hammerTargets = CustomTriggers.ConeRaycast(UnityEngine.Camera.main.transform, 30, range, 100);

                foreach (var target in hammerTargets) {
                    if (target.layer      == groundLayer) hasGround = true;
                    else if (target.layer == enemyLayer) {
                        hasEnemy = true;
                        target.GetComponent<HealthSystem>()?.TakeDamage(damage, WeaponDamageType.Hammer);
                    } else if (target.CompareTag("Soul Crystal")) {
                        hasCrystal = true;
                        target.GetComponent<SoulCollector>().Explode();
                    } else if (target.CompareTag("Breakable")) target.GetComponent<BreakableObject>().Break();
                }

                if (hasEnemy) {
                    rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                    rb.AddForce(
                        (Vector3.up + 3f * enemyBounceMultiplier * orientation.forward) * hammerForce,
                        ForceMode.Impulse
                    );
                    SoundFXManager.Instance.PlayRandom(hammerImpactSounds);
                    CameraPositioning.Instance.InduceStress(0.2f);
                    FrameHang.Instance.ExecFrameHang(hammer.basicFreezeFrame, 0.15f);
                } else if (hasGround) {
                    rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                    rb.AddForce(
                        (Vector3.up + orientation.forward * 3f) * hammerForce,
                        ForceMode.Impulse
                    );
                    Vector3 groundVFXPos = hammerRaycast.Raycast();
                    Instantiate(hammerVFX, groundVFXPos, Quaternion.identity);
                    SoundFXManager.Instance.PlayRandom(hammerImpactSounds);
                    CameraPositioning.Instance.InduceStress(0.2f);
                    FrameHang.Instance.ExecFrameHang(hammer.basicFreezeFrame, 0.01f);
                } else if (hasCrystal) {
                    rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                    rb.AddForce(
                        (Vector3.up + orientation.forward * -3f) * hammerForce,
                        ForceMode.Impulse
                    );
                    CameraPositioning.Instance.InduceStress(0.3f);
                    FrameHang.Instance.ExecFrameHang(hammer.basicFreezeFrame, 0.2f);
                }
            } else CancelCharge();

            StopCoroutine(storedCoroutine);
            Reset();
        }

        public void Reset() {
            isCharged  = false;
            isCharging = false;
            if (hammerChargeBar) hammerChargeBar.gameObject.SetActive(false);
        }
    }
}