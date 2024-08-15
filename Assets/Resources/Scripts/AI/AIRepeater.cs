using System.Collections.Generic;
using System.Threading;

namespace TEE.AI {
    public class AIRepeater : AINode {
        readonly int milliseconds;
        Timer        timer;

        public AIRepeater(int evalsPerSecond, List<AINode> children) : base(children) {
            milliseconds = 1000 / evalsPerSecond;
        }

        public override NodeState Evaluate() {
            timer = new Timer(state => {
                foreach (var child in Children) {
                    switch (child.Evaluate()) {
                        case NodeState.Failure:
                            timer.Dispose();
                            break;
                        default:
                            continue;
                    }
                }
            }, null, 0, milliseconds);

            return NodeState.Running;
        }
    }
}