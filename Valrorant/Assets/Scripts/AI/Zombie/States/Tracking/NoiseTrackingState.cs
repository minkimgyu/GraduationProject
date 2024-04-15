using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using AI;
using System;

namespace AI.ZombieFSM
{
    public class NoiseTrackingState : State
    {
        Action<Zombie.ActionState> SetState;
        Func<Vector3> ReturnFrontNoise;
        Func<bool> IsQueueEmpty;
        Action ClearAllNoise;

        Func<bool> IsTargetInSight;

        Action<Vector3, bool> FollowPath;
        Func<bool> IsFollowingFinish;

        Vector3 _noisePos;

        public NoiseTrackingState(ZombieBlackboard blackboard, Action<Zombie.ActionState> SetState)
        {
            IsTargetInSight = blackboard.IsTargetInSight;

            this.SetState = SetState;
            ReturnFrontNoise = blackboard.ReturnFrontNoise;
            IsQueueEmpty = blackboard.IsQueueEmpty;
            ClearAllNoise = blackboard.ClearAllNoise;

            FollowPath = blackboard.FollowPath;
            IsFollowingFinish = blackboard.IsFollowingFinish;
        }

        public override void OnStateEnter()
        {
            Debug.Log("NoiseTrackingState");
            _noisePos = ReturnFrontNoise();
        }

        public override void CheckStateChange()
        {
            bool isInSight = IsTargetInSight();
            if (isInSight == false) return;

            SetState?.Invoke(Zombie.ActionState.TargetFollowing);
        }

        public override void OnStateUpdate()
        {
            FollowPath(_noisePos, false);

            bool isFinish = IsFollowingFinish();
            if (isFinish == false) return;

            bool isEmpty = IsQueueEmpty();

            if (isEmpty == true)
            {
                SetState?.Invoke(Zombie.ActionState.Idle);
                return;
            }

            _noisePos = ReturnFrontNoise();
        }

        public override void OnStateExit()
        {
            ClearAllNoise?.Invoke();
        }
    }
}