using System.Collections;
using UnityEngine;

public class FrameHang : MonoBehaviour {

    bool waiting;

public static FrameHang Instance { get; private set; }

void Awake() {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    public void ExecFrameHang(float duration) {

        if (waiting)
            return;

        Time.timeScale = 0.0f;
        StartCoroutine(FrameHanging(duration));

    }

    IEnumerator FrameHanging(float duration) {

        waiting = true;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1.0f;
        waiting = false;


    }
}