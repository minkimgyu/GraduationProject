using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BehaviorTree.Nodes
{
    public class IsCloseToTarget : EvaluatingDistance
    {
        Func<ISightTarget> ReturnTargetInSight;

        public IsCloseToTarget(Transform myTransform, float closeDistance, float closeDistanceOffset, Func<ISightTarget> ReturnTargetInSight)
            : base(myTransform, closeDistance, closeDistanceOffset, false)
        {
            this.ReturnTargetInSight = ReturnTargetInSight;
        }

        public override NodeState Evaluate()
        {
            ISightTarget target = ReturnTargetInSight();
            if (target == null) return NodeState.FAILURE;

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