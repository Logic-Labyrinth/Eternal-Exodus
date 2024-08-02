using UnityEngine;

namespace TEE.Health {
    public class HealthBar : MonoBehaviour {
        Material            material;
        Transform           playerTransform;
        static readonly int ShaderPropertyProgress = Shader.PropertyToID("_Progress");

        void Awake() {
            playerTransform = Camera.main.transform;
            material        = GetComponent<Renderer>().materials[0];
            material.SetFloat(ShaderPropertyProgress, 1);
        }

        void OnEnable() {
            material.SetFloat(ShaderPropertyProgress, 1);
        }

        public void SetProgress(float progress) {
            material.SetFloat(ShaderPropertyProgress, progress);
        }

        void LateUpdate() {
            transform.LookAt(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z));
            transform.Rotate(Vector3.right, -90);
            transform.Rotate(Vector3.up,    180);
        }
    }
}