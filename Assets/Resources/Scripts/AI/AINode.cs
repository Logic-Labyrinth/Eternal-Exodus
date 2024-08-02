using System.Collections.Generic;

namespace TEE.AI {
    public enum NodeState {
        Running,
        Success,
        Failure
    }

    public class AINode {
        protected AINode Parent;

        // protected NodeState state;
        protected readonly List<AINode> Children = new();

        readonly Dictionary<string, object> data = new();

        public AINode() {
            Parent = null;
        }

        public AINode(List<AINode> children) {
            foreach (var child in children) Attach(child);
        }

        void Attach(AINode child) {
            child.Parent = this;
            Children.Add(child);
        }

        public virtual NodeState Evaluate() => NodeState.Failure;

        public void SetData(string key, object value) {
            data[key] = value;
        }

        public object GetData(string key) {
            if (data.TryGetValue(key, out var value)) return value;

            var node = Parent;
            while (node != null) {
                value = node.GetData(key);
                if (value != null) return value;
                node = node.Parent;
            }

            return value;
        }

        public bool ClearData(string key) {
            var cleared = false;
            if (data.ContainsKey(key)) {
                data.Remove(key);
                return true;
            }

            var node = Parent;
            while (node != null) {
                cleared = node.ClearData(key);
                if (cleared) return true;
                node = node.Parent;
            }

            return cleared;
        }
    }
}