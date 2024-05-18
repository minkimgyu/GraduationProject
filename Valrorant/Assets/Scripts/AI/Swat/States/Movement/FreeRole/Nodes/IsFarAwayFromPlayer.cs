using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BehaviorTree.Nodes
{
    public class IsFarAwayFromPlayer : EvaluatingDistance
    {
        Func<Vector3> ReturnPlayerPos;

        public IsFarAwayFromPlayer(Transform myTransform, float farDistance, float farDistanceOffset, Func<Vector3> ReturnPlayerPos, Action OnOutOfRangeRequested)
           : base(myTransform, farDistance, farDistanceOffset, true)
        {
            this.ReturnPlayerPos = ReturnPlayerPos;
            this.OnOutOfRangeRequested = OnOutOfRangeRequested;
        }

        public IsFarAwayFromPlayer(Transform myTransform, float farDistance, float farDistanceOffset, Func<Vector3> ReturnPlayerPos)
            : base(myTransform, farDistance, farDistanceOffset, true)
        {
            this.ReturnPlayerPos = ReturnPlayerPos;
        }

        public override NodeState Evaluate()
        {
            Vector3 targetPos = ReturnPlayerPos();
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

    public class IsFarAwayToTarget : EvaluatingDistance
    {
        Func<bool> IsTargetInSmallSight;
        Func<ISightTarget> ReturnTargetInSight;

        public IsFarAwayToTarget(Transform myTransform, float farDistance, float farDistanceOffset, Func<bool> IsTargetInSmallSight, Func<ISightTarget> ReturnTargetInSight)
            : base(myTransform, farDistance, farDistanceOffset, true)
        {
            this.IsTargetInSmallSight = IsTargetInSmallSight;
            this.ReturnTargetInSight = ReturnTargetInSight;
        }

        public override NodeState Evaluate()
        {
            bool isInSight = IsTargetInSmallSight();
            if (isInSight == false) return NodeState.FAILURE;

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