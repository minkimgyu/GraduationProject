using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AI;

namespace BehaviorTree.Nodes
{
    public class StickToPlayer : Node
    {
        Func<ISightTarget> ReturnPlayer;
        Func<Vector3, int, Vector3> ReturnNodePos;
        Action<Vector3, bool> FollowPath;

        Action<Vector3> View;
        Func<FormationData> ReturnFormationData;

        Transform _myTransform;
        float _radius;

        public StickToPlayer(Transform myTransform, float radius, Func<ISightTarget> ReturnPlayer, Func<Vector3, int, Vector3> ReturnNodePos,
            Action<Vector3, bool> FollowPath, Action<Vector3> View, Func<FormationData> ReturnFormationData)
        {
            _myTransform = myTransform;
            _radius = radius;

            this.ReturnPlayer = ReturnPlayer;

            this.ReturnNodePos = ReturnNodePos;
            this.FollowPath = FollowPath;
            this.ReturnFormationData = ReturnFormationData;
        }

        Vector3 ReturnCirclePos()
        {
            ISightTarget target = ReturnPlayer();

            Vector3 playerPos = target.ReturnPos();

            FormationData data = ReturnFormationData();

            float offset = 360f / data.MaxCount;

            float angle = offset * data.Index;

            float angleInRadians = angle * Mathf.Deg2Rad;
            float x = playerPos.x + _radius * Mathf.Cos(angleInRadians);
            float z = playerPos.z + _radius * Mathf.Sin(angleInRadians);

            return new Vector3(x, playerPos.y, z);
        }

        public override NodeState Evaluate()
        {
            // 공격 중 후퇴의 경우 View를 돌리지 않음
            Vector3 circlePos = ReturnCirclePos();
            Vector3 _targetPos = ReturnNodePos.Invoke(circlePos, 0);
            FollowPath?.Invoke(_targetPos, false);

            //ISightTarget target = ReturnPlayer();
            //Vector3 dir = (_myTransform.position - target.ReturnPos()).normalized;

            //View?.Invoke(new Vector3(dir.x, 0, dir.z));
            return NodeState.SUCCESS;
        }
    }
}