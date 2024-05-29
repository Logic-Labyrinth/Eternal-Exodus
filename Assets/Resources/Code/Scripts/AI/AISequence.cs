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
                        state = NodeState.FAILURE;
                        return state;
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.RUNNING:
                        anyChildIsRunning = true;
                        continue;
                    default:
                        state = NodeState.SUCCESS;
                        return state;
                }
            }
            state = anyChildIsRunning ? NodeState.RUNNING : NodeState.SUCCESS;
            return state;
        }
    }
}
