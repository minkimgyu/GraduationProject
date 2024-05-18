using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FSM;

namespace BehaviorTree.Nodes
{
    public class Follow : Node
    {
        Func<ITarget> ReturnTargetInSight;
        Action<Vector3, List<Vector3>, bool> FollowPath;

        public Follow(Func<ITarget> ReturnTargetInSight, Action<Vector3, List<Vector3>, bool> FollowPath)
        {
            this.ReturnTargetInSight = ReturnTargetInSight;
            this.FollowPath = FollowPath;
        }

        public override NodeState Evaluate()
        {
            ITarget target = ReturnTargetInSight();
            Vector3 pos = target.ReturnPos();
            FollowPath?.Invoke(pos, null, false);

            return NodeState.SUCCESS;
        }
    }
}