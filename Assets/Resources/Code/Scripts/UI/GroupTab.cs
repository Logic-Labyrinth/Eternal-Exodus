using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupTag : MonoBehaviour {
    public List<ButtonTab> buttonTabs;
    public Sprite tabIdle;
    public Sprite tabHover;
    public Sprite tabActive;

    public void Subscribe(ButtonTab button) {
        if (buttonTabs == null) {
            buttonTabs = new List<ButtonTab>();
        }
        buttonTabs.Add(button);
    }

    public void OnTabEnter(ButtonTab button) {
        ResetTabs();
        button.background.sprite = tabHover;
    }

    public void OnTabExit(ButtonTab button) {
        ResetTabs();
    }

    public void OnTabSelected(ButtonTab button) {
        ResetTabs();
        button.background.sprite = tabActive;
    }

    public void ResetTabs() {
        foreach (ButtonTab button in buttonTabs) {
            if (button) button.background.sprite = tabIdle;
        }
    }
}
