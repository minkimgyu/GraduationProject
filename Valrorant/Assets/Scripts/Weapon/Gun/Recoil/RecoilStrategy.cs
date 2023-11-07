using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObserverPattern;

abstract public class RecoilStrategy
{
    protected Vector2 _lastPosition;

    protected Vector2 _viewRotationMultiplier;
    protected Vector2 _pointToMove;

    protected float _shootInterval;

    protected float _cameraRecoilMultiplier = 0.5f;
    protected float _firePointRecoilMultiplier = 1.0f;
    protected float _actorBoneRecoilMultiplier = 0.5f;

    protected Timer _timer;

    public System.Action<Vector2, Vector2, Vector2> OnRecoilProgressRequested;

    public RecoilStrategy()
    {
        _timer = new Timer();
    }

    public abstract void RecoverRecoil();

    public abstract void ResetValues();

    public abstract void OnLink(GameObject player);

    public abstract void OnUnlink(GameObject player);

    public abstract void OnInintialize(GameObject player);

    // 마우스에 손을 때는 경우 반동 회복 시퀀스 작동
    // Create는 이밴트에 따라 작동하는 위치가 다르지만
    // Recover은 같으므로 여기에 넣자

    public abstract void OnEventRequested(); // 이거는 Auto 반동에 적용
    public abstract void OnEventFinished(); // 이거는 Auto 반동에 적용

    public abstract void OnClickStart(); // 이거는 Manual 반동에 적용

    public abstract void OnClickEnd();

    /// <summary>
    /// 다른 리코일 패턴이 실행된 경우
    /// </summary>
    public abstract void OnOtherActionEventRequested();
    
    public virtual void OnUpdate()
    {
        _timer.Update();
        ApplyMultiplier();
    }

    protected void StartLerp(Vector2 pointToMove, float duration)
    {
        _timer.Start(duration);
        _pointToMove = pointToMove;
    }

    protected void StopRecoil()
    {
        _timer.Reset();
    }

    protected Vector2 LerpRecoil()
    {
        if (_timer.IsFinish == true) StopRecoil();
        else _viewRotationMultiplier = Vector2.Lerp(_viewRotationMultiplier, _pointToMove, _timer.Ratio); // 변경되는 값을 넘겨주기

        _lastPosition = _viewRotationMultiplier;

        return _viewRotationMultiplier;
    }

    protected virtual void ApplyMultiplier()
    {
        if (_timer.IsRunning == false) return;

        Vector2 recoilValue = LerpRecoil();
        OnRecoilProgressRequested?.Invoke(recoilValue * _cameraRecoilMultiplier, recoilValue * _firePointRecoilMultiplier, recoilValue * _actorBoneRecoilMultiplier);
    }

    protected abstract Vector2 ReturnNextRecoilPoint();
}
