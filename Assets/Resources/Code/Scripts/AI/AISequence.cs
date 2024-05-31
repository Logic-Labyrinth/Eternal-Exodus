using System.Collections.Generic;

namespace BehaviorTree {
    public class AISequence : AINode {
        public AISequence() : base() { }
        public AISequence(List<AINode> children) : base(children) { }

        public override NodeState Evaluate() {
            bool anyChildIsRunning = false;
            foreach (AINode child in children) {
                switch (child.Evaluate()) {
                    case NodeState.FAILURE:
                        return NodeState.FAILURE;
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.RUNNING:
                        anyChildIsRunning = true;
                        continue;
                    default:
                        return NodeState.SUCCESS;
                }
            }

            return anyChildIsRunning ? NodeState.RUNNING : NodeState.SUCCESS;
        }
    }
}
