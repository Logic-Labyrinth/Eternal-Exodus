using UnityEngine;

public class LevelSelectMenuController : MonoBehaviour {
    GameManager gameManager;
    Animator animator;

    public static LevelSelectMenuController Instance { get; private set; }

    void Awake() {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
        if (!gameManager) gameManager = GameManager.Instance;
        animator = GetComponent<Animator>();
    }

    // public void ClickLeftButton() {
    //     animator.SetTrigger("Click Left Button");
    // }

    // public void ClickRightButton() {
    //     animator.SetTrigger("Click Right Button");
    // }
}
