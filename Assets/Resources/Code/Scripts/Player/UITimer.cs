using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;

public class UITimer : MonoBehaviour {
    public static UITimer Instance;

    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float countdownTimeSeconds = 120f;
    [SerializeField] float secondsPerPawn = 5;
    [SerializeField] float secondsPerRook = 5;
    [SerializeField] float secondsPerBishop = 5;


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

    public void ResetTime() {
        timer = 0;
    }

    //IEnumerable TornadoSizeReset(){



   // }

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
