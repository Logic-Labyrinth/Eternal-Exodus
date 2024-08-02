using UnityEngine;
using UnityEngine.EventSystems;

namespace TEE.UI.Tooltip {
    public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        // editable text box
        [SerializeField, Multiline(10)] string text;

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
            Tooltip.Instance.ShowTooltip(text);
        }

        public void OnPointerExit(PointerEventData eventData) {
            if (eventData.fullyExited)
                Tooltip.Instance.HideTooltip();
        }
    }
}