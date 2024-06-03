using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    // editable text box
    [SerializeField, Multiline(10)] string text;

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
        // Debug.Log("OnMouseOver");
        Tooltip.Instance.ShowTooltip(text);
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (!eventData.fullyExited) return;
        // Debug.Log("OnMouseExit");
        Tooltip.Instance.HideTooltip();
    }
}