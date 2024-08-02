using System.Collections.Generic;

namespace TEE.AI {
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
                NodeState.Success => trueNode.Evaluate(),
                NodeState.Failure => falseNode.Evaluate(),
                _ => NodeState.Running,
            };
        }
    }
}
