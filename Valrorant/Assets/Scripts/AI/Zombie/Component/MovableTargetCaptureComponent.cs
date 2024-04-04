using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class MovableTargetCaptureComponent<T> : BaseCaptureComponent<T> where T : ITarget
{
    protected List<T> _capturedTargets = new List<T>();

    public override void Initialize(float radius)
    {
        _captureRadius = radius;
    }

    protected override void OnTargetEnter(T target)
    {
        _capturedTargets.Add(target);
    }

    protected override void OnTargetExit(T target)
    {
        _capturedTargets.Remove(target);
    }

    protected override bool IsAlreadyContaining(T target)
    {
        return _capturedTargets.Contains(target);
    }
}
