using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using AI;
using System;

public class NoiseTrackingState : State
{
    Action<Zombie.State> SetState;
    Func<Vector3> ReturnFrontNoise;
    Func<bool> IsQueueEmpty;
    Action ClearAllNoise;

    Func<bool> IsTargetInSight;

    Action<Vector3, bool> FollowPath;
    Func<bool> IsFollowingFinish;

    Vector3 _noisePos;

    public NoiseTrackingState(NoiseTrackingStateParameter parameter)
    {
        IsTargetInSight = parameter.IsTargetInSight;

        SetState = parameter.SetState;
        ReturnFrontNoise = parameter.ReturnFrontNoise;
        IsQueueEmpty = parameter.IsQueueEmpty;
        ClearAllNoise = parameter.ClearAllNoise;

       FollowPath = parameter.FollowPath;
       IsFollowingFinish = parameter.IsFollowingFinish;
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

        SetState?.Invoke(Zombie.State.TargetFollowing);
    }

    public override void OnStateUpdate()
    {
        FollowPath(_noisePos, false);

        bool isFinish = IsFollowingFinish();
        if (isFinish == false) return;

        bool isEmpty = IsQueueEmpty();

        if (isEmpty == true)
        {
            SetState?.Invoke(Zombie.State.Idle);
            return;
        }

        _noisePos = ReturnFrontNoise();
    }

    public override void OnStateExit()
    {
        ClearAllNoise?.Invoke();
    }
}
