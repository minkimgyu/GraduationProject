using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BehaviorTree.Nodes
{
    public class AimTowardTarget : Node
    {
        Transform _aimTarget;

        Func<ITarget> ReturnTargetInSight;

        public AimTowardTarget(Transform aimTarget, Func<ITarget> ReturnTargetInSight)
        {
            _aimTarget = aimTarget;
            this.ReturnTargetInSight = ReturnTargetInSight;
        }

        public override NodeState Evaluate()
        {
            ITarget target = ReturnTargetInSight();
            _aimTarget.position = target.ReturnPos();

            return NodeState.SUCCESS;
        }
    }
}