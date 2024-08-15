using TMPro;
using UnityEngine;
using UnityEngine.VFX;

namespace TEE.Player {
    public class UITimer : MonoBehaviour {
        float                  timer;
        static TextMeshProUGUI timerText;
        static VisualEffect    tornadoVFX;
        static GameObject      tornadoTrigger;
        static Vector3         tornadoTriggerScale;

        protected void Awake() {
            tornadoVFX          = GameObject.Find("VG_Tornado").GetComponent<VisualEffect>();
            tornadoTrigger      = GameObject.Find("VG_Tornado_Trigger");
            tornadoTriggerScale = tornadoTrigger.transform.localScale;
        }

        public static void UpdateTornado(float time, float scale) {
            tornadoVFX.SetFloat("Size", scale * 100);
            tornadoTrigger.transform.localScale = tornadoTriggerScale * scale;

            timerText.text = (int)time / 60 + ":" + ((int)time % 60).ToString("D2");
        }
    }
}