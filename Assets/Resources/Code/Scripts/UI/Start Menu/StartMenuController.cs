using UnityEngine;

public class StartMenuController : MonoBehaviour {
    [SerializeField] Animator animator;
    [SerializeField] GameObject levelSelectMenu;
    public bool canClick = false;

    void Awake() {
        if (!animator) animator = transform.GetComponent<Animator>();
    }

    void Update() {
        animator.SetBool("canClick", canClick);

        if (animator.GetBool("canClick")) {
            if (Input.GetButtonDown("Basic Attack") || Input.GetButtonDown("Jump")) {
                animator.SetBool("hasClicked", true);
                levelSelectMenu.SetActive(true);
                gameObject.SetActive(false);
            }
        }
    }
}
