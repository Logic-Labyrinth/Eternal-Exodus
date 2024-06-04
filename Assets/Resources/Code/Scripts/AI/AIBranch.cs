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
            return checkNode.Evaluate() switch {
                NodeState.SUCCESS => trueNode.Evaluate(),
                NodeState.FAILURE => falseNode.Evaluate(),
                _ => NodeState.RUNNING,
            };
        }
    }
}
