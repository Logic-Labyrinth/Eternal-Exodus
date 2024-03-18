using UnityEngine;

public class StartMenuController : MonoBehaviour {
    [SerializeField] Animator animator;
    [SerializeField] GameObject mainMenu;
    public bool canClick = false;

    GameManager gameManager;

    void Start() {
        if (!animator) {
            animator = transform.GetComponent<Animator>();
        }

        gameManager = GameManager.Instance;
    }

    void Update() {
        animator.SetBool("canClick", canClick);

        if (animator.GetBool("canClick")) {
            if (Input.GetButtonDown("Basic Attack") || Input.GetButtonDown("Jump")) {
                animator.SetBool("hasClicked", true);
                // mainMenu.SetActive(true);
                gameManager.StartGame();
                gameObject.SetActive(false);
            }
        }
    }
}
