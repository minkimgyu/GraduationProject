using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BehaviorTree.Nodes
{
    public class RotateTowardTarget : Node
    {
        Transform _sightPoint;
        Func<ITarget> ReturnTargetInSight;
        Action<Vector3> View;

        public RotateTowardTarget(Transform sightPoint, Func<ITarget> ReturnTargetInSight, Action<Vector3> View)
        {
            _sightPoint = sightPoint;
            this.ReturnTargetInSight = ReturnTargetInSight;
            this.View = View;
        }

        public override NodeState Evaluate()
        {
            ITarget target = ReturnTargetInSight();
            Vector3 targetPos = target.ReturnPos();

            Vector3 dir = (targetPos - _sightPoint.position).normalized;
            View?.Invoke(dir);

            return NodeState.SUCCESS;
        }
    }
}
