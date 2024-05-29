using System.Collections.Generic;

namespace BehaviorTree {
    public enum NodeState {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    public class AINode {
        public AINode parent;

        protected NodeState state;
        protected List<AINode> children = new();

        readonly Dictionary<string, object> data = new();

        public AINode() {
            parent = null;
        }

        public AINode(List<AINode> children) {
            foreach (AINode child in children) {
                Attach(child);
            }
        }

        void Attach(AINode child) {
            child.parent = this;
            children.Add(child);
        }

        public virtual NodeState Evaluate() => NodeState.FAILURE;

        public void SetData(string key, object value) {
            data[key] = value;
        }

        public object GetData(string key) {
            if (data.TryGetValue(key, out object value)) return value;

            AINode node = parent;
            while (node != null) {
                value = node.GetData(key);
                if (value != null) return value;
                node = node.parent;
            }

            return value;
        }

        public bool ClearData(string key) {
            bool cleared = false;
            if (data.ContainsKey(key)) {
                data.Remove(key);
                return true;
            }

            AINode node = parent;
            while (node != null) {
                cleared = node.ClearData(key);
                if (cleared) return true;
                node = node.parent;
            }

            return cleared;
        }
    }
}
