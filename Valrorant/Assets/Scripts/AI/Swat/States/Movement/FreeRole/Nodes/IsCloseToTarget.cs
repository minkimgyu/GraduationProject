using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BehaviorTree.Nodes
{
    public class IsCloseToTarget : EvaluatingDistance
    {
        Func<bool> IsTargetInSight;
        Func<ISightTarget> ReturnTargetInSight;

        public IsCloseToTarget(Transform myTransform, float closeDistance, float closeDistanceOffset, 
            Func<bool> IsTargetInSight, Func<ISightTarget> ReturnTargetInSight)

            : base(myTransform, closeDistance, closeDistanceOffset, false)
        {
            this.IsTargetInSight = IsTargetInSight;
            this.ReturnTargetInSight = ReturnTargetInSight;
        }

        public override NodeState Evaluate()
        {
            bool isInSight = IsTargetInSight();
            if (isInSight == false) return NodeState.FAILURE;

            ISightTarget target = ReturnTargetInSight();
            SwitchState(target.ReturnPos());

            switch (_state)
            {
                case State.WithinRange:
                    return NodeState.SUCCESS;

                case State.OutOfRange:
                    return NodeState.FAILURE;

                default:
                    return NodeState.FAILURE;
            }
        }
    }
}