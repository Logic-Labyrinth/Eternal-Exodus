using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameHang : MonoBehaviour {

    bool waiting;

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