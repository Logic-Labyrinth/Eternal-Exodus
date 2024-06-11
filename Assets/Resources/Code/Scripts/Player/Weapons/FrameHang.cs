using System.Collections;
using UnityEngine;

public class FrameHang : MonoBehaviour {
    public static FrameHang Instance { get; private set; }

    bool waiting;

    void Awake() {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    public void ExecFrameHang(BasicFreezeFrame basicFreezeFrame, float duration, float delay = 0) {
        if (waiting) return;

        StartCoroutine(FrameHanging(basicFreezeFrame, duration, delay));
    }

    IEnumerator FrameHanging(BasicFreezeFrame basicFreezeFrame, float duration, float delay = 0) {
        waiting = true;
        float timer = 0f;

        if (delay != 0) yield return new WaitForSecondsRealtime(delay);

        Time.timeScale = 0.0f;
        while (timer < duration) {

            Time.timeScale = basicFreezeFrame.Evaluate(timer / duration);
            timer += Time.unscaledDeltaTime;
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }

        waiting = false;
        Time.timeScale = 1.0f;
    }
}
