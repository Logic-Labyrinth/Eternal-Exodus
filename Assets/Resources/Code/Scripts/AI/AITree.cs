using UnityEngine;

namespace BehaviorTree {
    public abstract class AITree : MonoBehaviour {
        AINode root = null;
        
        void Start() {
            root = SetupTree();
        }

        void FixedUpdate() {
            root?.Evaluate();
        }

        protected abstract AINode SetupTree();
    }
}
