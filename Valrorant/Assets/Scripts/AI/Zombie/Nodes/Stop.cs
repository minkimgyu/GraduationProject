using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BehaviorTree.Nodes
{
    public class Stop : Node
    {
        Action StopAgent;

        public Stop(Action Stop)
        {
            this.StopAgent = Stop;
        }

        public override NodeState Evaluate()
        {
            StopAgent?.Invoke();
            return NodeState.SUCCESS;
        }
    }
}