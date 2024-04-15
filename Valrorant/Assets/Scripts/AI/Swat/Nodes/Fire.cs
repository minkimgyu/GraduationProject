using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BehaviorTree.Nodes
{
    public class Fire : Node
    {
        public Fire()
        {
        }

        public override NodeState Evaluate()
        {
            return NodeState.SUCCESS;
        }
    }
}