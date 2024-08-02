using System.Collections.Generic;

namespace TEE.AI {
    public class AISequence : AINode {
        public AISequence() : base() { }
        public AISequence(List<AINode> children) : base(children) { }

        public override NodeState Evaluate() {
            var anyChildIsRunning = false;
            foreach (var child in Children) {
                switch (child.Evaluate()) {
                    case NodeState.Failure:
                        return NodeState.Failure;
                    case NodeState.Success:
                        continue;
                    case NodeState.Running:
                        anyChildIsRunning = true;
                        continue;
                    default:
                        return NodeState.Success;
                }
            }

            return anyChildIsRunning ? NodeState.Running : NodeState.Success;
        }
    }
}