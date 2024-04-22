using System.Linq;
using UnityEditor;
using UnityEngine;

public class simulateEditorPhysics : MonoBehaviour {
    void Awake() {
        // Physics.simulationMode = SimulationMode.Script;
        Physics.simulationMode = SimulationMode.FixedUpdate;
    }

    [MenuItem("Tools/Simulate Physics in Editor - Start x1")]
    static void EditorSimStart1() {
        Debug.Log("Simulating");
        Physics.simulationMode = SimulationMode.Script;

        FindObjectsOfType<Rigidbody>()
            .ToList()
            .ForEach(r => r.velocity = Vector3.zero);

        Time.timeScale = 1f;
        // Physics.simulationMode = SimulationMode.FixedUpdate;
        EditorApplication.update += Update;
    }

    [MenuItem("Tools/Simulate Physics in Editor - Start x10")]
    static void EditorSimStart2() {
        Debug.Log("Simulating");
        Physics.simulationMode = SimulationMode.Script;

        FindObjectsOfType<Rigidbody>()
            .ToList()
            .ForEach(r => r.velocity = Vector3.zero);

        Time.timeScale = 0.1f;
        // Physics.simulationMode = SimulationMode.FixedUpdate;
        EditorApplication.update += Update;
    }

    [MenuItem("Tools/Simulate Physics in Editor - Start x100")]
    static void EditorSimStart3() {
        Debug.Log("Simulating");
        Physics.simulationMode = SimulationMode.Script;

        FindObjectsOfType<Rigidbody>()
            .ToList()
            .ForEach(r => r.velocity = Vector3.zero);

        Time.timeScale = 0.01f;
        // Physics.simulationMode = SimulationMode.FixedUpdate;
        EditorApplication.update += Update;
    }

    [MenuItem("Tools/Simulate Physics in Editor - Stop")]
    static void EditorSimStop() {
        Debug.Log("Not Simulating");
        // Physics.simulationMode = SimulationMode.FixedUpdate;
        EditorApplication.update -= Update;
    }

    [MenuItem("Tools/Simulate Physics in Editor - Normal")]
    static void EditorSimNormal() {
        Debug.Log("Normal Simulation");
        Physics.simulationMode = SimulationMode.FixedUpdate;
        EditorApplication.update -= Update;
    }

    static void Update() {
        Physics.Simulate(Time.deltaTime);
    }
}