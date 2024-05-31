using System.Collections.Generic;
using UnityEngine;

public class ChasePointCreation : MonoBehaviour {
    [Range(0, 16)] public int pointCount = 4;
    public float pointRadius = 5f;
    public float enemyRadius = 5f;

    public List<GameObject> Points { get; private set; } = new();
    public static ChasePointCreation Instance { get; private set; }

    void Awake() {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        float angle = 360f / pointCount;
        for (int i = 0; i < pointCount; i++) {
            Vector3 pointLocation = transform.forward;
            pointLocation = Quaternion.AngleAxis(angle * i, Vector3.up) * pointLocation;
            pointLocation *= pointRadius;

            GameObject point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            point.transform.parent = transform;
            point.transform.localPosition = pointLocation;
            point.transform.localScale = Vector3.one * 0.5f;
            point.name = "Chase Point " + i;
            Destroy(point.GetComponent<SphereCollider>());

            Points.Add(point);
        }
    }
}
