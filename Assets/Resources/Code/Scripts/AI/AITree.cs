using UnityEngine;

namespace BehaviorTree {
    public abstract class AITree : MonoBehaviour {
        AINode root = null;
        bool isActive = true;

        void Start() { root = SetupTree(); }

        void FixedUpdate() {
            if (!isActive) return;
            root?.Evaluate();
        }

        protected abstract AINode SetupTree();

        public void SetActive(bool active) { isActive = active; }
    }
}
