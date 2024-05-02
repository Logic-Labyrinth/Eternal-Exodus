using TMPro;
using UnityEngine;

public class UITimer : MonoBehaviour {
    [SerializeField] TextMeshProUGUI timerText;

    void Update() {
        timerText.text = ((int)Time.timeSinceLevelLoad / 60).ToString() + ":" + ((int)Time.timeSinceLevelLoad % 60).ToString("D2");
    }
}
