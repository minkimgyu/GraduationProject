using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BehaviorTree.Nodes
{
    public class Retreat : Node
    {
        Func<ISightTarget> ReturnPlayer;
        Func<Vector3, int, Vector3> ReturnNodePos;
        Action<Vector3, bool> FollowPath;

        bool _faceRoute;

        public Retreat(Func<ISightTarget> ReturnPlayer, Func<Vector3, int, Vector3> ReturnNodePos, 
            Action<Vector3, bool> FollowPath, bool faceRoute)
        {
            this.ReturnPlayer = ReturnPlayer;

            this.ReturnNodePos = ReturnNodePos;
            this.FollowPath = FollowPath;

            _faceRoute = faceRoute;
        }

        public override NodeState Evaluate()
        {
             // 공격 중 후퇴의 경우 View를 돌리지 않음

            Vector3 playerPos = ReturnPlayer().ReturnPos();
            Vector3 _targetPos = ReturnNodePos.Invoke(playerPos, 0);
            FollowPath?.Invoke(_targetPos, _faceRoute);

            return NodeState.SUCCESS;
        }
    }
}