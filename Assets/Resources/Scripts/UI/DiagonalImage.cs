using UnityEngine;
using UnityEngine.UI;

namespace TEE.UI {
    public class DiagonalImage : MonoBehaviour {
        Image theButton;

        void Awake() {
            theButton = GetComponent<Image>();
        }

        void Start() {
            theButton.alphaHitTestMinimumThreshold = 0.5f;
        }
    }
}