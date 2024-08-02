using UnityEngine;
using UnityEngine.Serialization;

namespace TEE.Player {
    public class PositionFollower : MonoBehaviour {
        public                                  Transform targetTransform;
        [FormerlySerializedAs("Offset")] public Vector3   offset;


        // Update is called once per frame
        void Update() {
            transform.position = targetTransform.position + offset;
        }
    }
}