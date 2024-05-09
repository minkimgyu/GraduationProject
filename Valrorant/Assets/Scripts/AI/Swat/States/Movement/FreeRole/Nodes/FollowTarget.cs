using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BehaviorTree.Nodes
{
    public class FollowTarget : Node
    {
        Func<ISightTarget> ReturnTargetInSight;
        Func<Vector3, int, Vector3> ReturnNodePos;
        Action<Vector3, bool> FollowPath;

        public FollowTarget(Func<ISightTarget> ReturnTargetInSight, Func<Vector3, int, Vector3> ReturnNodePos, 
            Action<Vector3, bool> FollowPath)
        {
            this.ReturnTargetInSight = ReturnTargetInSight;

            this.ReturnNodePos = ReturnNodePos;
            this.FollowPath = FollowPath;
        }

        public override NodeState Evaluate()
        {
            // ���� �� ������ ��� View�� ������ ����
            ISightTarget target = ReturnTargetInSight();
            Vector3 _targetPos = ReturnNodePos.Invoke(target.ReturnPos(), 0);
            FollowPath?.Invoke(_targetPos, true);

            return NodeState.SUCCESS;
        }
    }
}