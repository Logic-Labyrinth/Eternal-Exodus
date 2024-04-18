using UnityEditor;
using UnityEngine;

public class simulateEditorPhysics : MonoBehaviour {
    void Awake(){
        // Physics.simulationMode = SimulationMode.Script;
        Physics.simulationMode = SimulationMode.FixedUpdate;
    }

    [MenuItem ("Tools/Simulate Physics in Editor - Start")]
    static void EditorSimStart () {
        Debug.Log("Simulating");
        Physics.simulationMode = SimulationMode.Script;
        // Physics.simulationMode = SimulationMode.FixedUpdate;
        EditorApplication.update += Update;
    }

    [MenuItem ("Tools/Simulate Physics in Editor - Stop")]
    static void EditorSimStop () {
        Debug.Log("Not Simulating");
        // Physics.simulationMode = SimulationMode.FixedUpdate;
        EditorApplication.update -= Update;
    }

    static void Update () {
        Physics.Simulate(Time.deltaTime);
    }
}