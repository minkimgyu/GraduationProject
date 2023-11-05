using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualRecoilGenerator : RecoilStrategy
{
    bool _nowCreateRecoil = false;
    RecoilRange _recoilRange;

    Vector2 _recoilDirection;

    public ManualRecoilGenerator(float shootInterval, float recoverDuration, RecoilRange recoilRange)
    {
        _shootInterval = shootInterval;
        _timer = new Timer();

        _recoilRange = recoilRange;
        _recoilDirection = Vector2.zero;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_nowCreateRecoil == true && _timer.IsFinish == true) // �ݵ��� �ɾ��ְ� ���� ���
        {
            StopRecoil();
            StartLerp(Vector2.zero, _recoilRange.RecoilRecoverDuration); // ����, ȸ����Ŵ
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
        _recoilDirection.Set(0, _recoilRange.YUpPoint);
        return _recoilDirection;
    }

    public override void OnEventFinished()
    {
    }

    public override void RecoverRecoil()
    {
        // �ݵ��� �ٷ� ȸ���ϴ� �ڵ� �ʿ���
        StopRecoil();
        StartLerp(Vector2.zero, _recoilRange.RecoilRecoverDuration);
    }

    public override void ResetRecoil()
    {
        _nowCreateRecoil = false;
        _timer.Reset();
    }
}
