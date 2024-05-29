using System.Collections.Generic;

namespace BehaviorTree {
    public class AISelector : AINode {
        public AISelector() : base() { }
        public AISelector(List<AINode> children) : base(children) { }

        public override NodeState Evaluate() {
            foreach (AINode child in children) {
                switch (child.Evaluate()) {
                    case NodeState.FAILURE:
                        continue;
                    case NodeState.SUCCESS:
                        state = NodeState.SUCCESS;
                        return state;
                    case NodeState.RUNNING:
                        state = NodeState.RUNNING;
                        return state;
                    default:
                        continue;
                }
            }
            state = NodeState.FAILURE;
            return state;
        }
    }
}
