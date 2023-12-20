using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRecoilGenerator : RecoilStrategy
{
    int _index = -1;
    int _indexMultiplier = 1;

    float _recoilRatio;

    RecoilMapData _recoilMap;
    List<Vector2> _recoilPoints;

    public AutoRecoilGenerator(float shootInterval, float recoilRatio, RecoilMapData recoilData)
    {
        _shootInterval = shootInterval;
        _timer = new Timer();

        _recoilRatio = recoilRatio;

        _recoilMap = recoilData;
        _recoilPoints = recoilData.ReturnAllAnglesBetweenCenterAndPoint();
        _actorBoneRecoilMultiplier = 1f;
    }

    public override void OnEventRequested()
    {
        StopRecoil();

        Vector2 point = ReturnNextRecoilPoint() * _recoilRatio;
        StartLerp(point, _shootInterval);
    }

    public override void OnClickEnd() // _viewRotationMultiplier 이거를 0으로 만들어주면 됨
    {
        StopRecoil();

        _indexMultiplier = 1;
        ResetIndex();
        StartLerp(Vector2.zero, _recoilMap.RecoveryDuration); // 
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
        RecoilReceiver viewComponent = player.GetComponent<RecoilReceiver>();
        if (viewComponent == null) return;

        OnRecoilProgressRequested += viewComponent.OnRecoilProgress;
    }

    public override void OnUnlink(GameObject player)
    {
        RecoilReceiver viewComponent = player.GetComponent<RecoilReceiver>();
        if (viewComponent == null) return;

        OnRecoilProgressRequested -= viewComponent.OnRecoilProgress;
    }

    public override void OnInintialize(GameObject player)
    {
        RecoilReceiver viewComponent = player.GetComponent<RecoilReceiver>();
        if (viewComponent == null) return;

        OnRecoilProgressRequested += viewComponent.OnRecoilProgress;
    }

    protected override Vector2 ReturnNextRecoilPoint()
    {
        _index += _indexMultiplier;

        if (_index >= _recoilPoints.Count - 1) // 끝지점에 도착한 경우
        {
            _indexMultiplier = -1;
        }
        else if (_indexMultiplier == -1 && _index == _recoilMap.RepeatIndex) // 반환 지점에서 _indexMultiplier가 -1인 경우
        {
            _indexMultiplier = 1;
        }

        return _recoilPoints[_index];
    }


    void ResetIndex() => _index = -1;

    public override void OnEventFinished()
    {
    }

    public override void RecoverRecoil()
    {
        StopRecoil();
        StartLerp(Vector2.zero, _recoilMap.RecoveryDuration);
    }

    public override void ResetValues()
    {
        _indexMultiplier = 1;
        ResetIndex();
    }
}
