using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BehaviorTree.Nodes
{
    public class RotateTowardTarget : Node
    {
        Transform _sightPoint;
        Func<ISightTarget> ReturnTargetInSight;
        Action<Vector3> View;

        public RotateTowardTarget(Transform sightPoint, Func<ISightTarget> ReturnTargetInSight, Action<Vector3> View)
        {
            _sightPoint = sightPoint;
            this.ReturnTargetInSight = ReturnTargetInSight;
            this.View = View;
        }

        public override NodeState Evaluate()
        {
            ISightTarget target = ReturnTargetInSight();
            Vector3 targetPos = target.ReturnPos();

            Vector3 dir = (targetPos - _sightPoint.position).normalized;
            dir = new Vector3(dir.x, 0, dir.z);

            View?.Invoke(dir);

            return NodeState.SUCCESS;
        }
    }
}
