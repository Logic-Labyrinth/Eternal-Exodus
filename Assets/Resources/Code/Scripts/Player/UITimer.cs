using TMPro;
using UnityEngine;

public class UITimer : MonoBehaviour {
    [SerializeField] TextMeshProUGUI timerText;

    void Update() {
        // timerText.text = Time.timeSinceLevelLoad.ToString("F2");
        //write the time in minutes and seconds
        timerText.text = ((int)Time.timeSinceLevelLoad / 60).ToString() + ":" + ((int)Time.timeSinceLevelLoad % 60).ToString("D2");
    }
}
