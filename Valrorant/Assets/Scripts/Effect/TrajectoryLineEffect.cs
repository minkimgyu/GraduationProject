using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrajectoryLineEffect : BaseEffect
{
    Vector3 _hitPosition;

    public override void Initialize(Vector3 hitPosition, Vector3 shootPosition)
    {
        transform.position = shootPosition;
        _hitPosition = hitPosition;
    }

    public override void PlayEffect()
    {
        _timer.Start(_duration);
    }

    void MoveTo(float progress)
    {
        transform.position = Vector3.Lerp(transform.position, _hitPosition, progress);
    }

    protected override void OnUpdate()
    {
        _timer.Update();

        if(_timer.IsRunning)
        {
            MoveTo(_timer.Ratio);
            return;
        }

        if(_timer.IsFinish)
        {
            DisableObject();
        }
    }
}
