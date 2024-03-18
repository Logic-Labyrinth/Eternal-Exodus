using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HammerAbility : MonoBehaviour {
    [SerializeField]
    private float chargeTime = 1f;

    [SerializeField]
    private BoxCollider impactArea;

    [SerializeField]
    private float hammerForce;

    [SerializeField]
    private float enemyBounceMultiplier;
    private LayerMask enemyLayer;
    private LayerMask groundLayer;
    private bool isCharging = false;
    private bool isCharged = false;
    private Rigidbody rb;
    private Transform orientation;
    private Coroutine storedCoroutine;

    public Slider slider;
    public Image handle,
        background;
    float timer = 0;

    void Start() {
        rb = transform.GetComponent<Rigidbody>();
        orientation = transform.Find("Orientation");
        enemyLayer = LayerMask.NameToLayer("Enemy");
        groundLayer = LayerMask.NameToLayer("Ground");
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
            // Check if impact area is colliding with either a ground layer or enemy layer
            if (
                Physics.CheckBox(
                    impactArea.transform.position,
                    impactArea.size * 0.5f,
                    impactArea.transform.rotation,
                    enemyLayer
                )
            ) {
                // Stop player y velocity whilst keeping the other velocity axes
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(
                    (Vector3.up + 3f * enemyBounceMultiplier * orientation.forward) * hammerForce,
                    ForceMode.Impulse
                );
            } else if (
                  Physics.CheckBox(
                      impactArea.transform.position,
                      impactArea.size * 0.5f,
                    //   Quaternion.identity,
                      impactArea.transform.rotation,
                      groundLayer
                  )
              ) {
                Debug.Log(impactArea.transform.position);
                Debug.Log(impactArea.size * 0.5f);
                Debug.Log("Ground Hit");
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(
                    (Vector3.up + orientation.forward * 3f) * hammerForce,
                    ForceMode.Impulse
                );
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
