using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class simulateEditorPhysics : MonoBehaviour {
    void Awake(){
        Physics.simulationMode = SimulationMode.Script;
    }

    [MenuItem ("Tools/Simulate Physics in Editor - Start")]
    static void editorSimStart () {
        Debug.Log("Simulating");
        Physics.simulationMode = SimulationMode.Script;
        EditorApplication.update += Update;
    }

    [MenuItem ("Tools/Simulate Physics in Editor - Stop")]
    static void editorSimStop () {
        Debug.Log("Not Simulating");
        EditorApplication.update -= Update;
    }

    static void Update () {
        Physics.Simulate(Time.fixedDeltaTime);
    }

}