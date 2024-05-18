using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AI;

public struct SwatMovementBlackboard
{
    public SwatMovementBlackboard(float angleOffset, float angleChangeAmount, int wanderOffset, float stateChangeDelay,
        Transform largetCaptureTransform, Transform myTransform, Transform sightPoint, Transform aimPoint,
        float farFromPlayerDistance, float farFromPlayerDistanceOffset, float closeDistance, float closeDistanceOffset,
        float farFromTargetDistance, float farFromTargetDistanceOffset, float formationRadius, float offset, float offsetChangeDuration,

        Action<Vector3, List<Vector3>, bool> FollowPath, Action<Vector3> View, Action Stop, Func<Vector3, int, Vector3> ReturnNodePos,
        Func<Vector3> ReturnPlayerPos, Func<FormationData> ReturnFormationData,


        Func<List<ISightTarget>> ReturnAllTargetInLargeSight,
        Func<bool> IsTargetInLargeSight, Func<ISightTarget> ReturnTargetInLargeSight, Action<float> ModifyLargeCaptureRadius,
        Func<bool> IsTargetInSmallSight, Func<ISightTarget> ReturnTargetInSmallSight, Action<float> ModifySmallCaptureRadius
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
        CaptureTransform = largetCaptureTransform;
        MyTransform = myTransform;
        SightPoint = sightPoint;
        AimPoint = aimPoint;

        FormationRadius = formationRadius;

        Offset = offset;
        OffsetChangeDuration = offsetChangeDuration;

        this.FollowPath = FollowPath;
        this.View = View;
        this.Stop = Stop;

        this.IsTargetInLargeSight = IsTargetInLargeSight;
        this.ReturnTargetInLargeSight = ReturnTargetInLargeSight;
        this.ModifyLargeCaptureRadius = ModifyLargeCaptureRadius;

        this.ReturnAllTargetInLargeSight = ReturnAllTargetInLargeSight;

        this.IsTargetInSmallSight = IsTargetInSmallSight;
        this.ReturnTargetInSmallSight = ReturnTargetInSmallSight;
        this.ModifySmallCaptureRadius = ModifySmallCaptureRadius;

        this.ReturnPlayerPos = ReturnPlayerPos;

        this.ReturnNodePos = ReturnNodePos;
        this.ReturnFormationData = ReturnFormationData;
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

    public float Offset { get; }
    public float OffsetChangeDuration { get; }


    public Transform CaptureTransform { get; }
    public Transform MyTransform { get; }
    public Transform SightPoint { get; }
    public Transform AimPoint { get; }

    public Action<Vector3, List<Vector3>, bool> FollowPath { get; }
    public Action<Vector3> View { get; }
    public Action Stop { get; }

    public Func<bool> IsTargetInLargeSight { get; }
    public Func<ISightTarget> ReturnTargetInLargeSight { get; }

    public Action<float> ModifyLargeCaptureRadius { get; }

    public Func<List<ISightTarget>> ReturnAllTargetInLargeSight { get; }

    public Func<bool> IsTargetInSmallSight { get; }
    public Func<ISightTarget> ReturnTargetInSmallSight { get; }

    public Action<float> ModifySmallCaptureRadius { get; }

    public Func<Vector3> ReturnPlayerPos { get; }

    public Func<Vector3, int, Vector3> ReturnNodePos { get; }
    public Func<FormationData> ReturnFormationData { get; }

    // FreeRole
}
