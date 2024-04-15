using System.Collections;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class HammerAbility : MonoBehaviour {
    [SerializeField] float chargeTime = 1f;
    [SerializeField] BoxCollider impactArea;
    [SerializeField] float hammerForce;
    [SerializeField] float enemyBounceMultiplier;
    [SerializeField] HammerRaycast hammerRaycast;
    [SerializeField] GameObject hammerVFX;

    LayerMask enemyLayer;
    LayerMask groundLayer;
    bool isCharging = false;
    bool isCharged = false;
    Rigidbody rb;
    Transform orientation;
    Coroutine storedCoroutine;

    public Slider slider;
    public Image handle, background;
    float timer = 0;

    GameObject DEBUG;

    void Start() {
        rb = transform.GetComponent<Rigidbody>();
        orientation = transform.Find("Orientation");
        enemyLayer = LayerMask.NameToLayer("Enemy");
        groundLayer = LayerMask.NameToLayer("Ground");
        DEBUG = GameObject.CreatePrimitive(PrimitiveType.Cube);
        DEBUG.GetComponent<BoxCollider>().enabled = false;
    }

    IEnumerator CompleteCharge() {
        yield return new WaitForSeconds(chargeTime);
        isCharged = isCharging;
    }

    public void ChargeHammer() {
        isCharging = true;
        storedCoroutine = StartCoroutine(CompleteCharge());
        timer = 0;
        handle.color = Color.white;
        background.color = Color.white;
        slider.value = 0;
        slider.gameObject.SetActive(true);
    }

    private void Update() {
        if (isCharging) {
            if (timer <= chargeTime) {
                slider.value = timer / chargeTime;
                timer += Time.deltaTime;
            } else {
                slider.value = 1;
                handle.color = Color.green;
                background.color = Color.green;
            }
        }
    }

    public void ActivateHammerAbility() {
        if (isCharged) {
            Collider[] colliders = Physics.OverlapBox(impactArea.transform.position + impactArea.center, impactArea.size * 0.5f, impactArea.transform.rotation);
            bool hasEnemy = false, hasGround = false;

            colliders.ForEach(x => {
                Debug.Log(x.gameObject.name);
                if (x.gameObject.layer == enemyLayer) hasEnemy = true;
                if (x.gameObject.layer == groundLayer) hasGround = true;
            });

            if (hasEnemy) {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(
                    (Vector3.up + 3f * enemyBounceMultiplier * orientation.forward) * hammerForce,
                    ForceMode.Impulse
                );
            } else if (hasGround) {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(
                    (Vector3.up + orientation.forward * 3f) * hammerForce,
                    ForceMode.Impulse
                );
                Vector3 groundVFXPos = hammerRaycast.Raycast();
                Instantiate(hammerVFX, groundVFXPos, Quaternion.identity);
            }
        }

        isCharged = false;
        isCharging = false;
        slider.gameObject.SetActive(false);
        StopCoroutine(storedCoroutine);
    }

    public void Reset() {
        isCharged = false;
        isCharging = false;
        if (slider) slider.gameObject.SetActive(false);
    }
}
