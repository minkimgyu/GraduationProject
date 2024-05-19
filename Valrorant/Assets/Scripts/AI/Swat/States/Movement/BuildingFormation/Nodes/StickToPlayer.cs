using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AI;

namespace BehaviorTree.Nodes
{
    public class StickToPlayer : Node
    {
        Func<Vector3> ReturnPlayerPos;
        Func<Vector3, int, Vector3> ReturnNodePos;
        Action<Vector3, List<Vector3>, bool> FollowPath;

        Func<FormationData> ReturnFormationData;
        float _radius;

        Vector3 _posOffset;

        float _offset = 3f;
        float _offsetChangeDuration = 5f;
        StopwatchTimer _timer;

        Func<List<ISightTarget>> ReturnAllTargetInLargeSight;

        public StickToPlayer(float radius, float offset, float offsetChangeDuration,  Func<Vector3> ReturnPlayerPos, Func<Vector3, int, Vector3> ReturnNodePos,
            Action<Vector3, List<Vector3>, bool> FollowPath, Action<Vector3> View, Func<FormationData> ReturnFormationData, Func<List<ISightTarget>> ReturnAllTargetInLargeSight)
        {
            _timer = new StopwatchTimer();
            _posOffset = Vector3.zero;

            _offset = offset;
            _offsetChangeDuration = offsetChangeDuration;
            _radius = radius;

            this.ReturnPlayerPos = ReturnPlayerPos;

            this.ReturnNodePos = ReturnNodePos;
            this.FollowPath = FollowPath;
            this.ReturnFormationData = ReturnFormationData;

            this.ReturnAllTargetInLargeSight = ReturnAllTargetInLargeSight;
        }

        Vector3 ReturnCirclePos()
        {
            Vector3 playerPos = ReturnPlayerPos();
            FormationData data = ReturnFormationData();

            float offset = 360f / data.MaxCount;

            float angle = offset * data.Index;

            float angleInRadians = angle * Mathf.Deg2Rad;
            float x = playerPos.x + _radius * Mathf.Cos(angleInRadians);
            float z = playerPos.z + _radius * Mathf.Sin(angleInRadians);

            return new Vector3(x, playerPos.y, z);
        }

        void ResetPosOffset()
        {
            _timer.Start(_offsetChangeDuration);
            if (_timer.CurrentState == StopwatchTimer.State.Finish)
            {
                float randomPosX = UnityEngine.Random.Range(-_offset, _offset);
                float randomPosZ = UnityEngine.Random.Range(-_offset, _offset);

                _posOffset = new Vector3(randomPosX, 0, randomPosZ);

                _timer.Reset();
                _timer.Start(_offsetChangeDuration);
            }
        }

        public override NodeState Evaluate()
        {
            ResetPosOffset();

            List<Vector3> nearHelperPos = new List<Vector3>();


            // isightTarget 넣어서 반영해줌
            FormationData data = ReturnFormationData();
            foreach (var listener in data.Listeners)
            {
                nearHelperPos.Add(listener.Value.ReturnPos());
            }

            Vector3 playerPos = ReturnPlayerPos();
            nearHelperPos.Add(playerPos);

            List<ISightTarget> targets = ReturnAllTargetInLargeSight();
            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i].IsUntrackable() == true) continue;

                nearHelperPos.Add(targets[i].ReturnPos());
            }

            // 공격 중 후퇴의 경우 View를 돌리지 않음
            Vector3 circlePos = ReturnCirclePos();
            Vector3 _targetPos = ReturnNodePos.Invoke(circlePos + _posOffset, 0);
            FollowPath?.Invoke(_targetPos, nearHelperPos, false);

            return NodeState.SUCCESS;
        }
    }
}