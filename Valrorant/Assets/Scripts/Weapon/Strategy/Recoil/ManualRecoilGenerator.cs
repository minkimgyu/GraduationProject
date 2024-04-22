using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualRecoilGenerator : RecoilGenerator
{
    Vector2 _recoilDirection;

    public ManualRecoilGenerator(float shootInterval, float recoveryDuration, RecoilRangeData recoilRange)
        : base(shootInterval, recoveryDuration)
    {
        _shootIntervalDuration = shootInterval;
        _recoilDirection = recoilRange.ReturnFixedPoint();
    }

    protected override Vector2 ReturnNextRecoilPoint() { return _recoilDirection; }



    //public override void OnOtherActionEventRequested()
    //{
    //    StopRecoil();
    //}

    //public override void OnEventStartRequested() { }

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

    //public override void OnEventEndRequested()
    //{
    //}



    //public override void OnActionFinishRequested()
    //{
    //}

    //public override void RecoverRecoil()
    //{
    //    // 반동을 바로 회복하는 코드 필요함
    //    StopRecoil();
    //    StartRecoil(Vector2.zero, _recoilRange.RecoveryDuration);
    //}

    //public override void ResetValues()
    //{
    //    _nowCreateRecoil = false;
    //    _timer.Reset();
    //}
}
