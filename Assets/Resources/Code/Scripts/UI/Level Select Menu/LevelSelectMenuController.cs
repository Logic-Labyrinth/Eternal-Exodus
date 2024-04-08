using UnityEngine;
using UnityEngine.EventSystems;

public class LevelSelectMenuController : MonoBehaviour {
    static GameObject buttonGO;
    GameManager gameManager;

    public static LevelSelectMenuController Instance { get; private set; }

    void Awake() {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
        if (!gameManager) gameManager = GameManager.Instance;
    }

    public static void OnPointerEnter(BaseEventData eventData) {
        buttonGO = (eventData as PointerEventData).pointerCurrentRaycast.gameObject;
        if (!buttonGO.CompareTag("Seed Select Button")) return;
        if (!buttonGO.TryGetComponent<UnityEngine.UI.Outline>(out var outline)) {
            Debug.Log("Outline not found on " + buttonGO);
            return;
        }
        outline.enabled = true;
    }

    public static void OnPointerExit(BaseEventData eventData) {
        if (!buttonGO.TryGetComponent<UnityEngine.UI.Outline>(out var outline)) {
            Debug.Log("Outline not found on " + buttonGO);
            return;
        }
        outline.enabled = false;
    }

    // public static void OnPointerClick(BaseEventData eventData) {
    //     buttonGO = (eventData as PointerEventData).pointerCurrentRaycast.gameObject;
    //     if (!buttonGO.CompareTag("Seed Select Button")) return;
    //     if (!buttonGO.TryGetComponent<SeedSelectButtonData>(out var seedData)) {
    //         Debug.Log("Seed data not found on " + buttonGO);
    //         return;
    //     }

    //     // GameManager.Instance.SetSeed(seedData.seed);
    //     // GameManager.Instance.StartGame();
    //     Instance.gameObject.SetActive(false);
    // }
}