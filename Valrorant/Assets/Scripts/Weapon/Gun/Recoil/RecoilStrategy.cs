using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObserverPattern;

abstract public class RecoilStrategy// : ISubject<Vector2, Vector2, Vector2>
{
    protected Vector2 _lastPosition;

    protected Vector2 _viewRotationMultiplier;
    protected Vector2 _pointToMove;

    protected float _shootInterval;
    protected float _recoverDuration;

    protected float _cameraRecoilMultiplier = 0.5f;
    protected float _firePointRecoilMultiplier = 1f;
    protected float _actorBoneRecoilMultiplier = 0.5f;

    protected Timer _timer;

    public System.Action<Vector2, Vector2, Vector2> OnRecoilProgressRequested;
    public System.Action OnRecoilStartRequested; // 다른 Recoil을 Cancel해줌

    public RecoilStrategy(float shootInterval, float recoverDuration)
    {
        _shootInterval = shootInterval;
        _recoverDuration = recoverDuration;
        _timer = new Timer();
    }

    public abstract void CreateRecoil(); // 이거 abstract로 남기기

    public abstract void RecoverRecoil(); // 이거 virtual로 남기기


    public virtual void OnUpdate()
    {
        _timer.Update();
        ApplyMultiplier();
    }

    protected void StartLerp(Vector2 pointToMove, float duration)
    {
        OnRecoilStartRequested?.Invoke();
        _timer.Start(duration);
        _pointToMove = pointToMove;
    }

    public void StopLerp()
    {
        _timer.Stop();
    }

    protected Vector2 LerpRecoil()
    {
        if (_timer.IsFinish == true) StopLerp();
        else _viewRotationMultiplier = Vector2.Lerp(_viewRotationMultiplier, _pointToMove, _timer.Ratio); // 변경되는 값을 넘겨주기

        _lastPosition = _viewRotationMultiplier;

        return _viewRotationMultiplier;
    }

    protected virtual void ApplyMultiplier()
    {
        if (_timer.IsFinish) return;

        Vector2 recoilValue = LerpRecoil();
        OnRecoilProgressRequested?.Invoke(recoilValue * _cameraRecoilMultiplier, recoilValue * _firePointRecoilMultiplier, recoilValue * _actorBoneRecoilMultiplier);
    }

    protected abstract Vector2 ReturnNextRecoilPoint();
}
