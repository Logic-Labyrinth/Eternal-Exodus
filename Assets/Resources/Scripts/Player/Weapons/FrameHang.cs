using System.Collections;
using LexUtils.Singleton;
using UnityEngine;

namespace TEE.Player.Weapons {
    public class FrameHang : Singleton<FrameHang> {
        bool waiting;

        public void ExecFrameHang(BasicFreezeFrame basicFreezeFrame, float duration, float delay = 0) {
            if (waiting) return;
            StartCoroutine(FrameHanging(basicFreezeFrame, duration, delay));
        }

        IEnumerator FrameHanging(BasicFreezeFrame basicFreezeFrame, float duration, float delay = 0) {
            waiting = true;
            float timer = 0f;

            if (delay != 0) yield return new WaitForSecondsRealtime(delay);

            while (timer < duration) {
                Time.timeScale =  basicFreezeFrame.Evaluate(timer / duration);
                timer          += Time.unscaledDeltaTime;
                yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
            }

            waiting        = false;
            Time.timeScale = 1.0f;
        }
    }
}