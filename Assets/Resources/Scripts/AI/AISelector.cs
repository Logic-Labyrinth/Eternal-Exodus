using System.Collections.Generic;

namespace TEE.AI {
    public class AISelector : AINode {
        public AISelector() : base() { }
        public AISelector(List<AINode> children) : base(children) { }

        public override NodeState Evaluate() {
            foreach (var child in Children) {
                switch (child.Evaluate()) {
                    case NodeState.Failure:
                        continue;
                    case NodeState.Success:
                        return NodeState.Success;
                    case NodeState.Running:
                        return NodeState.Running;
                    default:
                        continue;
                }
            }

            return NodeState.Failure;
        }
    }
}
