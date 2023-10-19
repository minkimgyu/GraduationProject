using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualRecoilGenerator : RecoilStrategy
{
    bool _nowCreateRecoil = false;
    RecoilRange _recoilRange;

    Vector2 _recoilDirection;

    public ManualRecoilGenerator(float shootInterval, float recoverTime, RecoilRange recoilRange) : base(shootInterval, recoverTime)
    {
        _recoilRange = recoilRange;
        _recoilDirection = Vector2.zero;
    }

    public override void CreateRecoil()
    {
        StopLerp();

        Vector2 point = ReturnNextRecoilPoint();

        _nowCreateRecoil = true;
        StartLerp(point, _shootInterval);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_nowCreateRecoil == true && _timer.IsFinish == true) // 반동을 걸어주고 끝난 경우
        {
            StopLerp();
            StartLerp(Vector2.zero, _recoverDuration); // 이후, 회복시킴
            _nowCreateRecoil = false;
        }
    }

    public override void RecoverRecoil()
    {
    }

    protected override Vector2 ReturnNextRecoilPoint()
    {
        _recoilDirection.Set(0, _recoilRange.YUpPoint);
        return _recoilDirection;
    }
}
