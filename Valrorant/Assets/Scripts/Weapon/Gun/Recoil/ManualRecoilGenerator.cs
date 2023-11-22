using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualRecoilGenerator : RecoilStrategy
{
    bool _nowCreateRecoil = false;
    RecoilRangeData _recoilRange;

    Vector2 _recoilDirection;

    public ManualRecoilGenerator(float shootInterval, RecoilRangeData recoilRange)
    {
        _shootInterval = shootInterval;
        _timer = new Timer();

        _recoilRange = recoilRange;
        _recoilDirection = recoilRange.ReturnFixedPoint();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_nowCreateRecoil == true && _timer.IsFinish == true) // 반동을 걸어주고 끝난 경우
        {
            StopRecoil();
            StartLerp(Vector2.zero, _recoilRange.RecoveryDuration); // 이후, 회복시킴
            _nowCreateRecoil = false;
        }
    }

    public override void OnOtherActionEventRequested()
    {
        StopRecoil();
    }

    public override void OnClickStart() { }

    public override void OnLink(GameObject player)
    {
        ViewComponent viewComponent = player.GetComponent<ViewComponent>();
        OnRecoilProgressRequested += viewComponent.OnRecoilProgress;
    }

    public override void OnUnlink(GameObject player)
    {
        ViewComponent viewComponent = player.GetComponent<ViewComponent>();
        OnRecoilProgressRequested -= viewComponent.OnRecoilProgress;
    }

    public override void OnInintialize(GameObject player)
    {
        ViewComponent viewComponent = player.GetComponent<ViewComponent>();
        OnRecoilProgressRequested += viewComponent.OnRecoilProgress;
    }

    public override void OnClickEnd()
    {
    }

    public override void OnEventRequested()
    {
        StopRecoil();

        Vector2 point = ReturnNextRecoilPoint();

        _nowCreateRecoil = true;
        StartLerp(point, _shootInterval);
    }

    protected override Vector2 ReturnNextRecoilPoint()
    {
        return _recoilDirection;
    }

    public override void OnEventFinished()
    {
    }

    public override void RecoverRecoil()
    {
        // 반동을 바로 회복하는 코드 필요함
        StopRecoil();
        StartLerp(Vector2.zero, _recoilRange.RecoveryDuration);
    }

    public override void ResetValues()
    {
        _nowCreateRecoil = false;
        _timer.Reset();
    }
}
