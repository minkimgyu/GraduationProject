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

    // ���콺�� ���� ���� ��� �ݵ� ȸ�� ������ �۵�
    // Create�� �̹�Ʈ�� ���� �۵��ϴ� ��ġ�� �ٸ�����
    // Recover�� �����Ƿ� ���⿡ ����

    public abstract void OnEventRequested(); // �̰Ŵ� Auto �ݵ��� ����
    public abstract void OnEventFinished(); // �̰Ŵ� Auto �ݵ��� ����

    public abstract void OnClickStart(); // �̰Ŵ� Manual �ݵ��� ����

    public abstract void OnClickEnd();

    /// <summary>
    /// �ٸ� ������ ������ ����� ���
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
        else _viewRotationMultiplier = Vector2.Lerp(_viewRotationMultiplier, _pointToMove, _timer.Ratio); // ����Ǵ� ���� �Ѱ��ֱ�

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
