using UnityEngine;

public class LevelSelectMenuController : MonoBehaviour {
    GameManager gameManager;

    public static LevelSelectMenuController Instance { get; private set; }

    void Awake() {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
        if (!gameManager) gameManager = GameManager.Instance;
    }
}
