using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstRecoilGenerator : RecoilGenerator
{
    RecoilRangeData _recoilRange;

    Vector2 _recoilDirection;
    Vector2 _originPoint;

    public BurstRecoilGenerator(float shootInterval, float recoveryDuration, RecoilRangeData recoilRange)
         : base(shootInterval, recoveryDuration)
    {
        _shootIntervalDuration = shootInterval;

        _recoilRange = recoilRange;

        _originPoint = Vector2.zero;
        _recoilDirection = _originPoint;
    }

    protected override Vector2 ReturnNextRecoilPoint()
    {
        Vector2 dir = _recoilRange.ReturnFixedPoint().V2;
        _recoilDirection += dir;
        return _recoilDirection;
    }

    protected override void OnStartRecovering() => _recoilDirection = _originPoint;

    //public override void OnEventEndRequested() // _viewRotationMultiplier 이거를 0으로 만들어주면 됨
    //{
    //}

    //public override void OnOtherActionEventRequested()
    //{
    //    StopRecoil();
    //}

    //public override void OnEventStartRequested()
    //{
    //}

    //public override void LinkEvent(GameObject player)
    //{
    //    RecoilReceiver viewComponent = player.GetComponent<RecoilReceiver>();
    //    OnRecoil += viewComponent.OnRecoilRequested;
    //}

    //public override void UnlinkEvent(GameObject player)
    //{
    //    RecoilReceiver viewComponent = player.GetComponent<RecoilReceiver>();
    //    OnRecoil -= viewComponent.OnRecoilRequested;
    //}

    //public override void OnInintialize(GameObject player)
    //{
    //    RecoilReceiver viewComponent = player.GetComponent<RecoilReceiver>();
    //    OnRecoil += viewComponent.OnRecoilRequested;
    //}



    //public override void OnActionFinishRequested()
    //{
    //    StopMultiply();
    //    _recoilDirection = _originPoint;
    //    StartMultiplying(Vector2.zero, _recoilRange.RecoveryDuration); // 이후, 회복시킴
    //}

    //public override void RecoverRecoil()
    //{
    //    StopRecoil();
    //    StartRecoil(Vector2.zero, _recoilRange.RecoveryDuration);
    //}

    //public override void ResetValues()
    //{
    //    StopRecoil();
    //    _recoilDirection = _originPoint;
    //}
}
