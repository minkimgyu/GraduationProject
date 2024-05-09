using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BehaviorTree.Nodes
{
    public class IsFarAwayToTarget : EvaluatingDistance
    {
        Func<ISightTarget> ReturnTargetInSight;

        public IsFarAwayToTarget(Transform myTransform, float farDistance, float farDistanceOffset, Func<ISightTarget> ReturnTargetInSight)
            : base(myTransform, farDistance, farDistanceOffset, true)
        {
            this.ReturnTargetInSight = ReturnTargetInSight;
        }

        public IsFarAwayToTarget(Transform myTransform, float farDistance, float farDistanceOffset, Func<ISightTarget> ReturnTargetInSight, Action OnOutOfRangeRequested)
           : base(myTransform, farDistance, farDistanceOffset, true)
        {
            this.ReturnTargetInSight = ReturnTargetInSight;
            this.OnOutOfRangeRequested = OnOutOfRangeRequested;
        }

        public override NodeState Evaluate()
        {
            ISightTarget target = ReturnTargetInSight();

            Vector3 targetPos = target.ReturnPos();
            SwitchState(targetPos);

            switch (_state)
            {
                case State.WithinRange:
                    return NodeState.FAILURE;

                case State.OutOfRange:
                    return NodeState.SUCCESS;

                default:
                    return NodeState.FAILURE;
            }
        }
    }
}