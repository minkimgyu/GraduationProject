using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct SwatMovementBlackboard
{
    public SwatMovementBlackboard(float angleOffset, float angleChangeAmount, int wanderOffset, float stateChangeDelay,
        Transform captureTransform, Transform myTransform, Transform sightPoint, Transform aimPoint, 
        float farFromPlayerDistance, float farFromPlayerDistanceOffset, float closeDistance, float closeDistanceOffset,
        float farFromTargetDistance, float farFromTargetDistanceOffset, float formationRadius,

        Action<Vector3, bool> FollowPath, Action<Vector3> View, Action Stop, Func<Vector3, int,  Vector3> ReturnNodePos, 
        Func<ISightTarget> ReturnPlayer, Func<bool> IsTargetInSight, Func<ISightTarget> ReturnTargetInSight
        )
    {
        FarFromPlayerDistance = farFromPlayerDistance;
        FarFromPlayerDistanceOffset = farFromPlayerDistanceOffset;

        FarFromTargetDistance = farFromTargetDistance;
        FarFromTargetDistanceOffset = farFromTargetDistanceOffset;

        CloseDistance = closeDistance;
        CloseDistanceOffset = closeDistanceOffset;

        AngleOffset = angleOffset;
        AngleChangeAmount = angleChangeAmount;
        WanderOffset = wanderOffset;
        StateChangeDelay = stateChangeDelay;
        CaptureTransform = captureTransform;
        MyTransform = myTransform;
        SightPoint = sightPoint;
        AimPoint = aimPoint;

        FormationRadius = formationRadius;

        this.FollowPath = FollowPath;
        this.View = View;
        this.Stop = Stop;

        this.IsTargetInSight = IsTargetInSight;
        this.ReturnTargetInSight = ReturnTargetInSight;
        this.ReturnPlayer = ReturnPlayer;

        this.ReturnNodePos = ReturnNodePos;
    }

    // FreeRole
    public float FarFromPlayerDistance { get; }
    public float FarFromPlayerDistanceOffset { get; }

    public float FarFromTargetDistance { get; }
    public float FarFromTargetDistanceOffset { get; }

    public float CloseDistance { get; }
    public float CloseDistanceOffset { get; }

    public float AngleOffset { get; }
    public float AngleChangeAmount { get; }
    public int WanderOffset { get; }
    public float StateChangeDelay { get; }

    public float FormationRadius { get; }


    public Transform CaptureTransform { get; }
    public Transform MyTransform { get; }
    public Transform SightPoint { get; }
    public Transform AimPoint { get; }

    public Action<Vector3, bool> FollowPath { get; }
    public Action<Vector3> View { get; }
    public Action Stop { get; }

    public Func<bool> IsTargetInSight { get; }
    public Func<ISightTarget> ReturnTargetInSight { get; }
    public Func<ISightTarget> ReturnPlayer { get; }

    public Func<Vector3, int, Vector3> ReturnNodePos { get; }

    // FreeRole
}
