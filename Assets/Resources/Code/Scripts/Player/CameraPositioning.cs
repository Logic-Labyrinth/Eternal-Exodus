using UnityEngine;

public class CameraPositioning : MonoBehaviour {
    [SerializeField] Transform trans;

    void Update() {
        transform.position = trans.position;
    }
}
