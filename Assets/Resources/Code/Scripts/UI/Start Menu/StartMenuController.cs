using UnityEngine;

public class StartMenuController : MonoBehaviour {
    [SerializeField] Animator animator;
    [SerializeField] GameObject levelSelectMenu;
    public bool canClick = false;

    void Awake() {
        if (!animator) animator = GetComponent<Animator>();
    }

    void Update() {
        // if (!canClick) return;

        if (Input.GetButtonDown("Basic Attack") || Input.GetButtonDown("Jump")) {
            animator.SetBool("hasClicked", true);
            levelSelectMenu.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
