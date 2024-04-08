using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FSM;

namespace BehaviorTree.Nodes
{
    public class RotateTowardTarget : Node
    {
        Transform _myTrasform;
        Action<Vector3> View;
        Func<ITarget> ReturnTargetInSight;

        public RotateTowardTarget(Transform myTrasform, Action<Vector3> View, Func<ITarget> ReturnTargetInSight)
        {
            _myTrasform = myTrasform;
            this.View = View;
            this.ReturnTargetInSight = ReturnTargetInSight;
        }

        public override NodeState Evaluate()
        {
            ITarget target = ReturnTargetInSight();
            Vector3 targetPos = target.ReturnPos();
            Vector3 dir = (targetPos - _myTrasform.position).normalized;

            View?.Invoke(dir);

            return NodeState.SUCCESS;
        }
    }
}
