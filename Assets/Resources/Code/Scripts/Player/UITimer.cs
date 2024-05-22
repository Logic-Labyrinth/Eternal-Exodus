using TMPro;
using UnityEngine;

public class UITimer : MonoBehaviour {
    public static UITimer Instance;

    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float countdownTimeSeconds = 300f;
    [SerializeField] int secondsPerPawn = 5;
    [SerializeField] int secondsPerRook = 5;
    [SerializeField] int secondsPerBishop = 5;
    // float scale = 1f;
    float timer = 0;

    void Awake() {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    void FixedUpdate() {
        timer += Time.fixedDeltaTime;
        float timeLeft = countdownTimeSeconds - timer;
        // scale = timeLeft / countdownTimeSeconds;
        timerText.text = ((int)timeLeft / 60).ToString() + ":" + ((int)timeLeft % 60).ToString("D2");
    }

    void AddTime(float time) {
        timer = Mathf.Max(0, timer - time);
    }

    public void AddPawnTime() {
        AddTime(secondsPerPawn);
    }

    public void AddRookTime() {
        AddTime(secondsPerRook);
    }

    public void AddBishopTime() {
        AddTime(secondsPerBishop);
    }
}
