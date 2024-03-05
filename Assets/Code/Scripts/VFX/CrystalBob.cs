using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class CrystalBob : MonoBehaviour {
    [SerializeField] float speed = 5f;
    [SerializeField] float height = 0.5f;
    Vector3 originalHeight;

    void Start() {
        originalHeight = transform.position;
    }
    void Update() {
        float y = Mathf.Sin(Time.time * speed) * height;
        Vector3 pos = originalHeight + new Vector3(0, y, 0);
        transform.position = pos;
    }
}