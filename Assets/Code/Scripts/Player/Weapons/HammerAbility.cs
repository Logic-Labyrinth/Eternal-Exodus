using System.Collections;
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
        enemyLayer = LayerMask.GetMask("Enemy");
        groundLayer = LayerMask.GetMask("Ground");
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

    // Update the charging progress if the charging flag is true
    private void Update() {
        if (isCharging) { // Check if the object is currently charging
            if (timer <= chargeTime) { // Check if the charging time has not exceeded the maximum charge time
                // Update the slider value based on the charging progress
                slider.value = timer / chargeTime;
                // Increment the timer based on the elapsed time
                timer += Time.deltaTime;
            } else {
                // Set the slider value to maximum
                slider.value = 1;
                // Set the handle and background color to green to indicate full charge
                handle.color = Color.green;
                background.color = Color.green;
            }
        }
    }

    public void ActivateHammerAbility() {
        Debug.Log(impactArea.size * 0.5f);
        if (isCharged) {
            // Check if impact area is colliding with either a ground layer or enemy layer
            if (
                Physics.CheckBox(
                    impactArea.transform.position,
                    impactArea.size * 0.5f * impactArea.transform.localScale.x,
                    Quaternion.identity,
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
                      impactArea.size * 0.5f * impactArea.transform.localScale.x,
                      Quaternion.identity,
                      groundLayer
                  )
              ) {
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
