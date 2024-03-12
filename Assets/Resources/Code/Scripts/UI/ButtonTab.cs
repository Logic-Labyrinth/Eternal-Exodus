using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ButtonTab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
    public GroupTag groupTab;
    public Image background;

    public void OnPointerClick(PointerEventData eventData) {
        Debug.Log("OnPointerClick");
        groupTab.OnTabSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        Debug.Log("OnPointerEnter");
        groupTab.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData) {
        Debug.Log("OnPointerExit");
        groupTab.OnTabExit(this);
    }

    private void Start() {
        background = GetComponent<Image>();
        groupTab.Subscribe(this);
    }
}
