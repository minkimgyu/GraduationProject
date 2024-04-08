using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AI;

public struct Blackboard
{
    public float MaxHP { get; }

    // Wander
    public float AngleOffset { get; }
    public float AngleChangeAmount { get; }
    public int WanderOffset { get; }
    public float StateChangeDelay { get; }
    public Transform CaptureTransform { get; }
    public Transform MyTrasform { get; }

    public Action<Vector3, bool> FollowPath { get; }
    public Action<Vector3> View { get; }
    public Action Stop { get; }

    public Func<bool> IsTargetInSight { get; }
    public Func<Vector3, int, Vector3> ReturnNodePos { get; }

    // Wander


    // Noise

    public Action ClearAllNoise { get; }
    public Func<bool> IsQueueEmpty { get; }
    public Func<Vector3> ReturnFrontNoise { get; }
    public Func<bool> IsFollowingFinish { get; }

    // Noise

    // Attack

    public float AdditiveCaptureRadius { get; }
    public float AdditiveAttackRange { get; }

    public float AttackRange { get; }
    public float AttackCircleRadius { get; }

    public float DelayDuration { get; }


    public LayerMask AttackLayer { get; }
    public Transform AttackPoint { get; }

    public Action<float> ModifyCaptureRadius { get; }

    public Action<string> ResetAnimatorTrigger { get; }
    public Action<string, bool> ResetAnimatorBool { get; }

    public Func<ITarget> ReturnTargetInSight { get; }

    // Attack

    public float DestoryDelay { get; }

    public Blackboard(float angleOffset, float angleChangeAmount, int wanderOffset, float stateChangeDelay, 
        Transform captureTransform, Transform myTrasform, float additiveCaptureRadius, float additiveAttackRange, 
        float attackRange, float attackCircleRadius, float delayDuration, LayerMask attackLayer, Transform attackPoint,
        float destoryDelay, float maxHP,

        Action<Vector3, bool> FollowPath, Action<Vector3> View, Action Stop, Func<bool> IsTargetInSight,
        Func<Vector3, int, Vector3> ReturnNodePos, Action ClearAllNoise, Func<bool> IsQueueEmpty, Func<Vector3> ReturnFrontNoise,
        Func<bool> IsFollowingFinish, Action<float> ModifyCaptureRadius, Action<string, bool> ResetAnimatorBool, Action<string> ResetAnimatorTrigger, 
        Func<ITarget> ReturnTargetInSight)
    {
        MaxHP = maxHP;

        AngleOffset = angleOffset;
        AngleChangeAmount = angleChangeAmount;
        WanderOffset = wanderOffset;
        StateChangeDelay = stateChangeDelay;
        CaptureTransform = captureTransform;
        MyTrasform = myTrasform;

        DestoryDelay = destoryDelay;

        this.FollowPath = FollowPath;
        this.View = View;
        this.Stop = Stop;

        this.ReturnNodePos = ReturnNodePos;

        AdditiveCaptureRadius = additiveCaptureRadius;
        AdditiveAttackRange = additiveAttackRange;
        AttackRange = attackRange;
        AttackCircleRadius = attackCircleRadius;
        DelayDuration = delayDuration;

        AttackLayer = attackLayer;
        AttackPoint = attackPoint;

        this.ModifyCaptureRadius = ModifyCaptureRadius;
        this.ResetAnimatorTrigger = ResetAnimatorTrigger;
        this.ResetAnimatorBool = ResetAnimatorBool;
        this.ReturnTargetInSight = ReturnTargetInSight;

        this.ClearAllNoise = ClearAllNoise;

        this.IsTargetInSight = IsTargetInSight;
        this.IsQueueEmpty = IsQueueEmpty;
        this.ReturnFrontNoise = ReturnFrontNoise;
        this.IsFollowingFinish = IsFollowingFinish;
    }
}
