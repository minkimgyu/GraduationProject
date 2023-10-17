using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : BaseEffect
{
    ParticleSystem _objectBurstEffect;

    float spaceBetweenWall = 0.001f;

    protected SmoothLerpRoutine runningRoutine;

    protected virtual void Awake()
    {
        _objectBurstEffect = GetComponentInChildren<ParticleSystem>();
        TryGetComponent(out runningRoutine);

        if (runningRoutine == null) return;

        runningRoutine.Initialize();
        runningRoutine.RoutineEnd += DisableObject;
    }

    public override void Initialize(Vector3 hitPosition, Vector3 hitNormal, Quaternion holeRotation)
    {
        transform.position = hitPosition + (hitNormal * spaceBetweenWall);
        transform.rotation = holeRotation * transform.rotation;
    }

    public override void PlayEffect()
    {
        _objectBurstEffect.Play();

        if (runningRoutine == null) return;
        runningRoutine.StartRoutine();
    }
}
