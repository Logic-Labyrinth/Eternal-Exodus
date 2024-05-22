using TMPro;
using UnityEngine;
using UnityEngine.VFX;

public class UITimer : MonoBehaviour {
    public static UITimer Instance;

    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float countdownTimeSeconds = 300f;
    [SerializeField] int secondsPerPawn = 5;
    [SerializeField] int secondsPerRook = 5;
    [SerializeField] int secondsPerBishop = 5;
    float scale = 1f;
    float timer = 0;
    VisualEffect tornadoVFX;
    GameObject tornadoTrigger;
    Vector3 tornadoTriggerScale;

    void Awake() {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
        tornadoVFX = GameObject.Find("VG_Tornado").GetComponent<VisualEffect>();
        tornadoTrigger = GameObject.Find("VG_Tornado_Trigger");
        tornadoTriggerScale = tornadoTrigger.transform.localScale;
    }

    void FixedUpdate() {
        timer += Time.fixedDeltaTime;
        float timeLeft = countdownTimeSeconds - timer;
        scale = timeLeft / countdownTimeSeconds;

        tornadoVFX.SetFloat("Size", scale * 100);
        tornadoTrigger.transform.localScale = tornadoTriggerScale * scale;
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
