using UnityEngine;

public class PlayerSliding : MonoBehaviour {
  [Header("Sliding")]
  [SerializeField] float maxSlideTime = 0.75f;
  [SerializeField] float slideForce = 200;
  [SerializeField] float slideYScale = 0.5f;
  float startYScale;
  float slideTimer;

  float horizontalInput;
  float verticalInput;
  bool isSliding;

  Transform orientation;
  Transform player;
  Rigidbody rb;
  PlayerMovement pm;

  private void Start() {
    orientation = transform.Find("Orientation");
    player = transform.Find("Player");
    rb = GetComponent<Rigidbody>();
    pm = GetComponent<PlayerMovement>();
    startYScale = player.localScale.y;
  }

  private void Update() {
    horizontalInput = Input.GetAxisRaw("Horizontal");
    verticalInput = Input.GetAxisRaw("Vertical");

    // if (Input.GetKeyDown(KeyCode.LeftControl) && (horizontalInput != 0 || verticalInput != 0))
    if (Input.GetKeyDown(KeyCode.LeftControl) && rb.velocity.magnitude > pm.crouchSpeed * 1.1f)
      StartSlide();

    if (Input.GetKeyUp(KeyCode.LeftControl) && isSliding)
      StopSlide();
  }

  void FixedUpdate() {
    if (isSliding) SlidingMovement();
  }

  void StartSlide() {
    isSliding = true;
    player.localScale = new Vector3(player.localScale.x, slideYScale, player.localScale.z);
    rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

    slideTimer = maxSlideTime;
  }

  void SlidingMovement() {
    Vector3 direction = orientation.forward * verticalInput + orientation.right * horizontalInput;

    if (!pm.OnSlope() || rb.velocity.y > -0.1f) {
      rb.AddForce(direction.normalized * slideForce, ForceMode.Force);
      slideTimer -= Time.deltaTime;
    } else {
      rb.AddForce(pm.GetSlopeMoveDirection(direction) * slideForce, ForceMode.Force);
    }

    if (slideTimer <= 0 || rb.velocity.y <= pm.crouchSpeed * 1.1f) StopSlide();
  }

  void StopSlide() {
    isSliding = false;
    player.localScale = new Vector3(player.localScale.x, startYScale, player.localScale.z);
  }
}
