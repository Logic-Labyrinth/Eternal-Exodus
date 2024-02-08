using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugUI : MonoBehaviour {
    float t = 0;
    public string stringToEdit = "Hello World";

    void OnGUI() {
        // GUILayout.Button("I am an Automatic Layout Button");
        // GUILayout.TextField("" + t);
        GUI.TextField(new Rect(10, 10, 200, 20), stringToEdit, 25);
        t += Time.deltaTime;
        stringToEdit = t + "";
    }
}
