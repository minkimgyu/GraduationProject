using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstRecoilGenerator : RecoilStrategy
{
    RecoilRange _recoilRange;

    float _upPoint;

    Vector2 _recoilDirection;

    public BurstRecoilGenerator(float shootInterval, RecoilRange recoilRange)
    {
        _shootInterval = shootInterval;
        _timer = new Timer();

        _upPoint = 0;

        _recoilRange = recoilRange;
        _recoilDirection = Vector2.zero;
    }

    public override void OnEventRequested()
    {
        StopRecoil();
        Vector2 point = ReturnNextRecoilPoint();
        StartLerp(point, _shootInterval);
    }

    public override void OnClickEnd() // _viewRotationMultiplier �̰Ÿ� 0���� ������ָ� ��
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
        _upPoint += _recoilRange.YUpPoint;

        _recoilDirection.Set(0, _upPoint);
        return _recoilDirection;
    }


    public override void OnEventFinished()
    {
        StopRecoil();
        _upPoint = 0; // �ʱ�ȭ
        StartLerp(Vector2.zero, _recoilRange.RecoilRecoverDuration); // ����, ȸ����Ŵ
    }

    public override void RecoverRecoil()
    {
        StopRecoil();
        StartLerp(Vector2.zero, _recoilRange.RecoilRecoverDuration);
    }

    public override void ResetRecoil()
    {
        StopRecoil();
        _upPoint = 0; // �ʱ�ȭ
    }
}
