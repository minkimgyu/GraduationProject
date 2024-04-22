using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRecoilGenerator : RecoilGenerator
{
    int _index = -1;
    int _indexMultiplier = 1;
    int _repeatIndex = 0;

    float _recoilRatio;
    List<Vector2> _recoilPoints;

    public AutoRecoilGenerator(float shootInterval, float recoveryDuration, float recoilRatio, RecoilMapData recoilData)
        : base(shootInterval, recoveryDuration)
    {
        _shootIntervalDuration = shootInterval;
        //_timer = new Timer();

        _recoilRatio = recoilRatio;
        _repeatIndex = recoilData.RepeatIndex;
        _recoilPoints = recoilData.ReturnAllAnglesBetweenCenterAndPoint();
        //_actorBoneRecoilMultiplier = 1f;
    }

    protected override Vector2 ReturnNextRecoilPoint()
    {
        Debug.Log(_index);

        _index += _indexMultiplier;

        if (_index >= _recoilPoints.Count - 1) _indexMultiplier = -1; // 끝지점에 도착한 경우
        else if (_indexMultiplier == -1 && _index == _repeatIndex) _indexMultiplier = 1;
        // 반환 지점에서 _indexMultiplier가 -1인 경우

        return _recoilPoints[_index] * _recoilRatio;
    }

    protected override void OnStartRecovering() { _index = -1; _indexMultiplier = 1; } // 인덱스 초기화 필요

    //// 따로 메소드 하나 더 만들던가 해줘야 할 듯
    //public override void OnEventEndRequested() // _viewRotationMultiplier 이거를 0으로 만들어주면 됨
    //{
    //    StopMultiply();

    //    _indexMultiplier = 1;
    //    _index = -1;
    //    StartMultiplying(Vector2.zero, _recoilMap.RecoveryDuration); // 
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
    //    if (viewComponent == null) return;

    //    OnRecoil += viewComponent.OnRecoilRequested;
    //}

    //public override void UnlinkEvent(GameObject player)
    //{
    //    RecoilReceiver viewComponent = player.GetComponent<RecoilReceiver>();
    //    if (viewComponent == null) return;

    //    OnRecoil -= viewComponent.OnRecoilRequested;
    //}

    //public override void OnInintialize(GameObject player)
    //{
    //    RecoilReceiver viewComponent = player.GetComponent<RecoilReceiver>();
    //    if (viewComponent == null) return;

    //    OnRecoil += viewComponent.OnRecoilRequested;
    //}


    //void ResetIndex() => _index = -1;

    //public override void OnActionFinishRequested()
    //{
    //}

    //public override void RecoverRecoil()
    //{
    //    StopRecoil();
    //    StartRecoil(Vector2.zero, _recoilMap.RecoveryDuration);
    //}

    //public override void ResetValues()
    //{
    //    _indexMultiplier = 1;
    //    ResetIndex();
    //}
}
