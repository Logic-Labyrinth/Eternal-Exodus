using System.Collections;
using UnityEngine;

public class FrameHang : MonoBehaviour {
    bool waiting;

    public static FrameHang Instance { get; private set; }

    void Awake() {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    public void ExecFrameHang(BasicFreezeFrame basicFreezeFrame, float duration) {
        if (waiting) return;

        Time.timeScale = 0.0f;
        StartCoroutine(FrameHanging(basicFreezeFrame, duration));
    }

    IEnumerator FrameHanging(BasicFreezeFrame basicFreezeFrame, float duration) {
        waiting = true;
        float timer = 0f;
        while (timer < duration) {

            Time.timeScale = basicFreezeFrame.Evaluate(timer / duration);
            timer += Time.deltaTime;
            yield return null;

        }

        waiting = false;
    }
}