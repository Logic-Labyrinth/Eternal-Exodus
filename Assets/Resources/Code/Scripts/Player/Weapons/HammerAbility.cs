using System.Collections;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class HammerAbility : MonoBehaviour {
    [SerializeField] float chargeTime = 1f;
    [SerializeField] BoxCollider impactArea;
    [SerializeField] float hammerForce;
    [SerializeField] float enemyBounceMultiplier;
    [SerializeField] HammerRaycast hammerRaycast;
    [SerializeField] GameObject hammerVFX;
    [SerializeField] Sound[] hammerImpactSounds;
    [SerializeField] Sound[] hammerChargeSounds;
    [SerializeField] Image hammerChargeBar;
    [SerializeField] Color chargeColor;
    Material hammerChargeBarMaterial;

    LayerMask enemyLayer;
    LayerMask groundLayer;
    bool isCharging = false;
    bool isCharged = false;
    Rigidbody rb;
    Transform orientation;
    Coroutine storedCoroutine;
    float timer = 0;

    void Start() {
        rb = transform.GetComponent<Rigidbody>();
        orientation = transform.Find("Orientation");
        enemyLayer = LayerMask.NameToLayer("Enemy");
        groundLayer = LayerMask.NameToLayer("Ground");
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
        timer = 0;
        hammerChargeBarMaterial.SetColor("_Color", chargeColor);
        hammerChargeBarMaterial.SetFloat("_Progress", 0);
        hammerChargeBar.gameObject.SetActive(true);
    }

    private void Update() {
        if (isCharging) {
            if (timer <= chargeTime) {
                hammerChargeBarMaterial.SetFloat("_Progress", timer / chargeTime);
                timer += Time.deltaTime;
            } else {
                hammerChargeBarMaterial.SetFloat("_Progress", 1);
                hammerChargeBarMaterial.SetColor("_Color", Color.white);
            }
        }
    }

    public void ActivateHammerAbility(int damage) {
        if (isCharged) {
            Collider[] colliders = Physics.OverlapBox(impactArea.bounds.center, impactArea.bounds.extents, impactArea.transform.rotation);
            bool hasEnemy = false, hasGround = false;

            colliders.ForEach(x => {
                if (x.gameObject.layer == enemyLayer) hasEnemy = true;
                if (x.gameObject.layer == groundLayer) hasGround = true;
                if (x.gameObject.layer == enemyLayer) {
                    hasEnemy = true;
                    x.GetComponent<HealthSystem>().TakeDamage(damage, WeaponDamageType.HAMMER, x.transform.position);
                }
            });

            if (hasEnemy) {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(
                    (Vector3.up + 3f * enemyBounceMultiplier * orientation.forward) * hammerForce,
                    ForceMode.Impulse
                );
                SoundFXManager.Instance.PlayRandom(hammerImpactSounds);
            } else if (hasGround) {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(
                    (Vector3.up + orientation.forward * 3f) * hammerForce,
                    ForceMode.Impulse
                );
                Vector3 groundVFXPos = hammerRaycast.Raycast();
                Instantiate(hammerVFX, groundVFXPos, Quaternion.identity);
                SoundFXManager.Instance.PlayRandom(hammerImpactSounds);
            }
        }

        StopCoroutine(storedCoroutine);
        Reset();
    }

    public void Reset() {
        isCharged = false;
        isCharging = false;
        if (hammerChargeBar) hammerChargeBar.gameObject.SetActive(false);
    }
}
