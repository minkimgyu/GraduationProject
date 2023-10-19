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

        if (_nowCreateRecoil == true && _timer.IsFinish == true) // �ݵ��� �ɾ��ְ� ���� ���
        {
            StopLerp();
            StartLerp(Vector2.zero, _recoverDuration); // ����, ȸ����Ŵ
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
