using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstRecoilGenerator : RecoilStrategy
{
    //RecoilRange _recoilRange;

    RecoilRangeData _recoilRange;

    Vector2 _recoilDirection;

    public BurstRecoilGenerator(float shootInterval, RecoilRangeData recoilRange)
    {
        _shootInterval = shootInterval;
        _timer = new Timer();

        _recoilRange = recoilRange;
        _recoilDirection = Vector2.zero;
    }

    public override void OnEventRequested()
    {
        StopRecoil();
        Vector2 point = ReturnNextRecoilPoint();
        StartLerp(point, _shootInterval);
    }

    public override void OnClickEnd() // _viewRotationMultiplier 이거를 0으로 만들어주면 됨
    {
    }

    public override void OnOtherActionEventRequested()
    {
        StopRecoil();
    }

    public override void OnClickStart()
    {
    }

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

    protected override Vector2 ReturnNextRecoilPoint()
    {
        _recoilDirection += _recoilRange.Point;
        return _recoilDirection;
    }


    public override void OnEventFinished()
    {
        StopRecoil();
        _recoilDirection = Vector2.zero;
        StartLerp(Vector2.zero, _recoilRange.RecoveryDuration); // 이후, 회복시킴
    }

    public override void RecoverRecoil()
    {
        StopRecoil();
        StartLerp(Vector2.zero, _recoilRange.RecoveryDuration);
    }

    public override void ResetValues()
    {
        StopRecoil();
        _recoilDirection = Vector2.zero;
    }
}
