using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AI;

public struct ZombieBlackboard
{
    // Wander
    public float AngleOffset { get; }
    public float AngleChangeAmount { get; }
    public int WanderOffset { get; }
    public float StateChangeDelay { get; }
    public Transform CaptureTransform { get; }
    public Transform MyTransform { get; }
    public Transform SightPoint { get; }

    public Action<Vector3, List<Vector3>, bool> FollowPath { get; }
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
    public float AttackDamage { get; }
    public float AttackCircleRadius { get; }

    public float DelayForNextAttack { get; }
    public float PreAttackDelay { get; }


    public LayerMask AttackLayer { get; }
    public Transform AttackPoint { get; }

    public Action<float> ModifyCaptureRadius { get; }

    public Action<string> ResetAnimatorTrigger { get; }
    public Action<string, bool> ResetAnimatorBool { get; }

    public Func<ISightTarget> ReturnTargetInSight { get; }

    // Attack

    public ZombieBlackboard(float attackDamage, float angleOffset, float angleChangeAmount, int wanderOffset, float stateChangeDelay, 
        Transform captureTransform, Transform myTrasform, Transform sightPoint, float additiveCaptureRadius, float additiveAttackRange, 
        float attackRange, float attackCircleRadius, float delayForNextAttack, float preAttackDelay, LayerMask attackLayer, Transform attackPoint,

        Action<Vector3, List<Vector3>, bool> FollowPath, Action<Vector3> View, Action Stop, Func<bool> IsTargetInSight,
        Func<Vector3, int, Vector3> ReturnNodePos, Action ClearAllNoise, Func<bool> IsQueueEmpty, Func<Vector3> ReturnFrontNoise,
        Func<bool> IsFollowingFinish, Action<float> ModifyCaptureRadius, Action<string, bool> ResetAnimatorBool, Action<string> ResetAnimatorTrigger, 
        Func<ISightTarget> ReturnTargetInSight)
    {
        AttackDamage = attackDamage;

        AngleOffset = angleOffset;
        AngleChangeAmount = angleChangeAmount;
        WanderOffset = wanderOffset;
        StateChangeDelay = stateChangeDelay;
        CaptureTransform = captureTransform;
        MyTransform = myTrasform;
        SightPoint = sightPoint;

        this.FollowPath = FollowPath;
        this.View = View;
        this.Stop = Stop;

        this.ReturnNodePos = ReturnNodePos;

        AdditiveCaptureRadius = additiveCaptureRadius;
        AdditiveAttackRange = additiveAttackRange;
        AttackRange = attackRange;
        AttackCircleRadius = attackCircleRadius;
        DelayForNextAttack = delayForNextAttack;
        PreAttackDelay = preAttackDelay;

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
