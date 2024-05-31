using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ChasePointCreation : MonoBehaviour {
    [Header("Inner Points")]
    [LabelText("Count"), Range(0, 16)] public int innerPointCount = 8;
    [LabelText("Radius")] public float innerPointRadius = 10f;
    [LabelText("Offset")] public float innerPointRadiusOffset = 1f;

    [Header("Outer Points")]
    [LabelText("Count"), Range(0, 16)] public int outerPointCount = 4;
    [LabelText("Radius")] public float outerPointRadius = 20f;
    [LabelText("Offset")] public float outerPointRadiusOffset = 1f;

    public List<GameObject> InnerPoints { get; private set; } = new();
    public List<GameObject> OuterPoints { get; private set; } = new();
    public static ChasePointCreation Instance { get; private set; }

    void Awake() {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        float angle = 360f / innerPointCount;
        for (int i = 0; i < innerPointCount; i++) {
            Vector3 pointLocation = transform.forward;
            pointLocation = Quaternion.AngleAxis(angle * i, Vector3.up) * pointLocation;
            pointLocation *= innerPointRadius;

            GameObject point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            point.transform.parent = transform;
            point.transform.localPosition = pointLocation;
            point.transform.localScale = Vector3.one * 0.5f;
            point.name = "Chase Point " + i;
            Destroy(point.GetComponent<SphereCollider>());

            InnerPoints.Add(point);
        }

        angle = 360f / outerPointCount;
        for (int i = 0; i < outerPointCount; i++) {
            Vector3 pointLocation = transform.forward;
            pointLocation = Quaternion.AngleAxis(angle * i, Vector3.up) * pointLocation;
            pointLocation *= outerPointRadius;

            GameObject point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            point.transform.parent = transform;
            point.transform.localPosition = pointLocation;
            point.transform.localScale = Vector3.one * 0.5f;
            point.name = "Chase Point " + i;
            Destroy(point.GetComponent<SphereCollider>());

            OuterPoints.Add(point);
        }
    }
}
