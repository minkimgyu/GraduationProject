using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AI;

public struct NoiseTrackingStateParameter
{
    public NoiseTrackingStateParameter(
        Action<Zombie.State> SetState,
        Action ClearAllNoise,
        Action<Vector3, bool> FollowPath,

        Func<bool> IsTargetInSight, 
        Func<bool> IsQueueEmpty, 
        Func<Vector3> ReturnFrontNoise, 
        Func<bool> IsFollowingFinish
        )
    {
        this.SetState = SetState;
        this.ClearAllNoise = ClearAllNoise;
        this.FollowPath = FollowPath;

        this.IsTargetInSight = IsTargetInSight;
        this.IsQueueEmpty = IsQueueEmpty;
        this.ReturnFrontNoise = ReturnFrontNoise;
        this.IsFollowingFinish = IsFollowingFinish;

    }

    public Action<Zombie.State> SetState { get; }
    public Action ClearAllNoise { get; }
    public Action<Vector3, bool> FollowPath { get; }


    public Func<bool> IsTargetInSight { get; }
    public Func<bool> IsQueueEmpty { get; }
    public Func<Vector3> ReturnFrontNoise { get; }
    public Func<bool> IsFollowingFinish { get; }
}
