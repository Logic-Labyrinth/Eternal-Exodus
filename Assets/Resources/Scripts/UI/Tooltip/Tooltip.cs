using LexUtils.Singleton;
using UnityEngine;
using TMPro;

namespace TEE.UI.Tooltip {
    public class Tooltip : Singleton<Tooltip> {
        public TextMeshProUGUI tooltipText;

        float         screenHeight;
        float         xDiff, yDiff;
        RectTransform rectTransform;

        protected override void Awake() {
            base.Awake();
            screenHeight = Screen.height;
            xDiff        = Screen.width  / 1920f;
            yDiff        = Screen.height / 1080f;

            gameObject.SetActive(false);
            rectTransform = GetComponent<RectTransform>();
        }

        public void ShowTooltip(string text) {
            Vector3 mousePos = Input.mousePosition;
            mousePos.y -= screenHeight;

            mousePos.x /= xDiff;
            mousePos.y /= yDiff;

            rectTransform.anchoredPosition = mousePos;

            tooltipText.text = text;
            gameObject.SetActive(true);
        }

        public void HideTooltip() {
            gameObject.SetActive(false);
        }

        void Update() {
            Vector3 mousePos = Input.mousePosition;
            mousePos.y -= screenHeight;

            mousePos.x /= xDiff;
            mousePos.y /= yDiff;
            mousePos   += new Vector3(25, -25, 0);

            rectTransform.anchoredPosition = mousePos;
        }
    }
}