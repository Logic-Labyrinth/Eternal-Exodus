using UnityEngine;
using TMPro;

public class Tooltip : MonoBehaviour {
    public TextMeshProUGUI tooltipText;
    public static Tooltip Instance { get; private set; }

    void Awake() {
        // create instance
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        // hide tooltip
        gameObject.SetActive(false);
    }

    public void ShowTooltip(string text) {
        transform.position = Input.mousePosition;
        tooltipText.text = text;
        gameObject.SetActive(true);
    }

    public void HideTooltip() {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        // follow mouse in canvas
        transform.position = Input.mousePosition;
    }
}
