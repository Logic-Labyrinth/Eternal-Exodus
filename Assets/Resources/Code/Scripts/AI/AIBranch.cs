using System.Collections.Generic;

namespace BehaviorTree {
    public class AIBranch : AINode {
        readonly AINode checkNode;
        readonly AINode trueNode;
        readonly AINode falseNode;

        public AIBranch() : base() { }
        public AIBranch(AINode checkNode, AINode trueNode, AINode falseNode) : base(new List<AINode> { checkNode, trueNode, falseNode }) {
            this.checkNode = checkNode;
            this.trueNode = trueNode;
            this.falseNode = falseNode;
        }

        public override NodeState Evaluate() {
            switch(checkNode.Evaluate()) {
                case NodeState.SUCCESS:
                    state = trueNode.Evaluate();
                    return state;
                case NodeState.FAILURE:
                    state = falseNode.Evaluate();
                    return state;
                default:
                    state = NodeState.RUNNING;
                    return state;
            }
        }
    }
}
