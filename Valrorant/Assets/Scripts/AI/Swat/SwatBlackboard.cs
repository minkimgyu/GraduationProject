using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct SwatBlackboard
{
    public SwatBlackboard(Transform myTransform, Transform sightPoint, Transform aimTarget, float attackRadius
        , float additiveAttackRadius, float delayDuration, Func<bool> IsTargetInSight,
        Func<ITarget> ReturnTargetInSight, Action<float> ModifyCaptureRadius)
    {
        MyTransform = myTransform;
        SightPoint = sightPoint;

        AimTarget = aimTarget;
        AttackRadius = attackRadius;
        AdditiveAttackRadius = additiveAttackRadius;
        DelayDuration = delayDuration;

        this.IsTargetInSight = IsTargetInSight;
        this.ReturnTargetInSight = ReturnTargetInSight;
        this.ModifyCaptureRadius = ModifyCaptureRadius;
    }

    public Action<float> ModifyCaptureRadius { get; }
    public Func<bool> IsTargetInSight { get; }
    public Func<ITarget> ReturnTargetInSight { get; }

    public Transform MyTransform { get; }
    public Transform SightPoint { get; }
    public Transform AimTarget { get; }

    public float AttackRadius { get; }
    public float AdditiveAttackRadius { get; }

    public float DelayDuration { get; }
}
