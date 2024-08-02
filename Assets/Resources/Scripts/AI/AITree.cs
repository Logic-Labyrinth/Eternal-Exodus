using UnityEngine;

namespace TEE.AI {
    public abstract class AITree : MonoBehaviour {
        AINode        root;
        float         updateInterval;
        float         elapsedTime;
        bool          isActive         = true;
        protected int UpdatesPerSecond = 0;

        void Start() {
            root = SetupTree();
            updateInterval = 1f / UpdatesPerSecond;
        }

        void FixedUpdate() {
            if (!isActive) return;
            if (UpdatesPerSecond == 0) return;

            elapsedTime += Time.deltaTime;
            if (elapsedTime < updateInterval) return;
            elapsedTime = 0;
            root?.Evaluate();
        }

        protected abstract AINode SetupTree();

        public void SetActive(bool active) => isActive = active;
    }
}