using UnityEngine;

public class PlayerSliding : MonoBehaviour {
  [Header("References")]
  public Transform orientation;
  public Transform playerObj;
  Rigidbody rb;
  PlayerMovement pm;

  [Header("Sliding")]
  [SerializeField] float slideForce = 200f;
  [SerializeField] float slideYScale = 0.5f;
  float startYScale;

  [Header("Input")]
  float horizontalInput;
  float verticalInput;

  private void Start() {
    rb = GetComponent<Rigidbody>();
    pm = GetComponent<PlayerMovement>();

    startYScale = playerObj.localScale.y;
  }

  private void Update() {
    horizontalInput = Input.GetAxisRaw("Horizontal");
    verticalInput = Input.GetAxisRaw("Vertical");

    if (Input.GetButtonDown("Crouch")) {
      pm.wantsToUncrouch = false;
      if (rb.velocity.magnitude > pm.crouchSpeed * 1.1f) StartSlide();
      else pm.StartCrouch();
    }

    if (Input.GetButtonUp("Crouch")) {
      if (pm.sliding) StopSlide();
      pm.StartCrouch();
      pm.wantsToUncrouch = true;
    }
  }

  private void FixedUpdate() {
    if (pm.sliding)
      SlidingMovement();
  }

  public void StartSlide() {
    pm.sliding = true;

    playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
    rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
    rb.AddForce(orientation.forward.normalized * 1000f, ForceMode.Force);
  }

  private void SlidingMovement() {
    Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

    if (!pm.OnSlope() || rb.velocity.y > -0.1f) rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);
    else rb.AddForce(pm.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);

    if (rb.velocity.magnitude <= pm.crouchSpeed) {
      StopSlide();
      pm.sliding = false;
      pm.StartCrouch();
    }
  }

  public void StopSlide() {
    pm.sliding = false;
    playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
  }
}
