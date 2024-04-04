using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BehaviorTree.Nodes
{
    public class StopAgent : Node
    {
        Action Stop;

        public StopAgent(Action Stop)
        {
            this.Stop = Stop;
        }

        public override NodeState Evaluate()
        {
            Stop?.Invoke();
            return NodeState.SUCCESS;
        }
    }

}