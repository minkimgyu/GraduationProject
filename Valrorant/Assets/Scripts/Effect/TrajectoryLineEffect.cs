using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryLineEffect : BaseEffect
{
    Vector3 _hitPosition;
    RunningRoutine runningRoutine;

    private void Awake()
    {
        TryGetComponent(out runningRoutine);

        if (runningRoutine == null) return;

        runningRoutine.Initialize();
        runningRoutine.RoutineAction += MoveTo;
        runningRoutine.RoutineEnd += DisableObject;
    }

    public override void Initialize(Vector3 hitPosition, Vector3 shootPosition)
    {
        transform.position = shootPosition;
        _hitPosition = hitPosition;
    }

    public override void PlayEffect()
    {
        if (runningRoutine == null) return;
        runningRoutine.StartRoutine();
    }

    void MoveTo(float progress)
    {
        transform.position = Vector3.Lerp(transform.position, _hitPosition, progress);
    }
}
