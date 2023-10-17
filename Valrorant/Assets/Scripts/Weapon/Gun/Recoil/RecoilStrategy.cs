using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObserverPattern;

abstract public class RecoilStrategy : ISubject<Vector2, Vector2, Vector2>
{
    Vector2 _storedMultiplier;

    protected Vector2 _viewRotationMultiplier;
    protected Vector2 _pointToMove;

    protected float _shootInterval;
    protected float _recoverDuration;

    private float _storedDuration = 0;
    private float _FinishDuration = 0;

    protected bool _canRunningLerp = false;

    protected float _cameraRecoilMultiplier = 0.5f;
    protected float _firePointRecoilMultiplier = 1f;
    protected float _actorBoneRecoilMultiplier = 0.5f;

    public RecoilStrategy(float shootInterval, float recoverDuration)
    {
        _shootInterval = shootInterval;
        _recoverDuration = recoverDuration;
        Observers = new List<IObserver<Vector2, Vector2, Vector2>>();
    }

    public abstract void CreateRecoil(); // 이거 abstract로 남기기

    public abstract void RecoverRecoil(); // 이거 virtual로 남기기


    public virtual void OnUpdate()
    {
        ApplyMultiplier();
    }

    protected void StartLerp(Vector2 pointToMove, float duration)
    {
        _FinishDuration = duration;
        _pointToMove = pointToMove;
        _canRunningLerp = true;
    }

    protected void StopLerp()
    {
        _canRunningLerp = false;
        _storedDuration = 0;
        _FinishDuration = 0;
    }

    protected Vector2 LerpRecoil()
    {
        _storedDuration += Time.deltaTime;
        Vector2 tmpValue;

        if (_storedDuration >= _FinishDuration)
        {
            _viewRotationMultiplier = _pointToMove;
            tmpValue = _viewRotationMultiplier - _pointToMove;
            StopLerp();
        }
        else
        {
            // 변경되는 값을 넘겨줘서 더하기
            Vector2 nowValue = Vector2.Lerp(_viewRotationMultiplier, _pointToMove, _storedDuration / _FinishDuration);

            tmpValue = nowValue - _storedMultiplier;

            _viewRotationMultiplier = nowValue;
            _storedMultiplier = nowValue;
        }

        return tmpValue;
    }

    protected virtual void ApplyMultiplier()
    {
        if (_canRunningLerp == false) return;

        Vector2 recoilValue = LerpRecoil();

        NotifyToObservers(recoilValue * _cameraRecoilMultiplier, recoilValue * _firePointRecoilMultiplier, recoilValue * _actorBoneRecoilMultiplier);
    }

    protected abstract Vector2 ReturnNextRecoilPoint();

    public List<IObserver<Vector2, Vector2, Vector2>> Observers { get; set; }

    public void AddObserver(IObserver<Vector2, Vector2, Vector2> observer)
    {
        Observers.Add(observer);
    }

    public void RemoveObserver(IObserver<Vector2, Vector2, Vector2> observer)
    {
        Observers.Remove(observer);
    }

    public void NotifyToObservers(Vector2 cameraRotationMultiplier, Vector2 firePointRotationMultiplier, Vector2 actorBoneRotationMultiplier)
    {
        for (int i = 0; i < Observers.Count; i++)
        {
            Observers[i].Notify(cameraRotationMultiplier, firePointRotationMultiplier, actorBoneRotationMultiplier);
        }
    }
}
