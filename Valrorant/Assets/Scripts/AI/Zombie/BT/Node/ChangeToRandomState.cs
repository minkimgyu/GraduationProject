using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

namespace BehaviorTree.Nodes
{
    public class ChangeToRandomState : Node
    {
        Func<WanderingFSM.State, bool> SetState;

        public ChangeToRandomState(Func<WanderingFSM.State, bool> SetState)
        {
            this.SetState = SetState;
        }

        public override NodeState Evaluate()
        {
            int count = Enum.GetValues(typeof(WanderingFSM.State)).Length;
            WanderingFSM.State newState = (WanderingFSM.State)Random.Range(0, count);

            SetState?.Invoke(newState);
            return NodeState.SUCCESS;
        }
    }
}
