using UnityEngine;
using TMPro;

public class Tooltip : MonoBehaviour {
    public TextMeshProUGUI tooltipText;
    public static Tooltip Instance { get; private set; }
    float screenHeight;

    void Awake() {
        // create instance
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
        screenHeight = Screen.height;

        // hide tooltip
        gameObject.SetActive(false);
    }

    public void ShowTooltip(string text) {
        Vector3 mousePos = Input.mousePosition;
        mousePos.y -= screenHeight;
        GetComponent<RectTransform>().anchoredPosition = mousePos;

        tooltipText.text = text;
        gameObject.SetActive(true);
    }

    public void HideTooltip() {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        // follow mouse in canvas
        Vector3 mousePos = Input.mousePosition;
        mousePos.y -= screenHeight;
        GetComponent<RectTransform>().anchoredPosition = mousePos;
    }
}
