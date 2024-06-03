using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // editable text box
    [SerializeField]
    [Multiline(10)]
    private string text;

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