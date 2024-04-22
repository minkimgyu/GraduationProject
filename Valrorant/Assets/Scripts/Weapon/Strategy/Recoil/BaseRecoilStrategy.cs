using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

abstract public class BaseRecoilStrategy : BaseStrategy
{
    public BaseRecoilStrategy() { }

    //public abstract void RecoverRecoil();

    //public abstract void ResetValues();

    //public abstract void LinkEvent(GameObject player);

    //public abstract void UnlinkEvent(GameObject player);

    //public abstract void OnInintialize(GameObject player);

    // ���콺�� ���� ���� ��� �ݵ� ȸ�� ������ �۵�
    // Create�� �̹�Ʈ�� ���� �۵��ϴ� ��ġ�� �ٸ�����
    // Recover�� �����Ƿ� ���⿡ ����

    /// <summary>
    /// Action �̺�Ʈ�� ȣ��Ǵ� Ÿ�ֿ̹� ����
    /// </summary>
    public virtual void Execute() { }

    /// <summary>
    /// Action �̺�Ʈ�� ����Ǵ� Ÿ�ֿ̹� ����
    /// </summary>
    //public virtual void OnActionFinishRequested() { }

    //public virtual void OnEventStartRequested() { } // �̰Ŵ� Manual �ݵ��� ����

    //public virtual void OnEventEndRequested() { }

    /// <summary>
    /// �ٸ� ������ ������ ����� ���
    /// </summary>
    //public abstract void OnOtherActionEventRequested();
}

abstract public class RecoilGenerator : BaseRecoilStrategy
{
    protected Vector2 _viewRotationMultiplier;
    protected Vector2 _goalMultiplier;

    //protected const float _cameraRecoilMultiplier = 0.5f;
    //protected const float _firePointRecoilMultiplier = 1.0f;
    //protected const float _actorBoneRecoilMultiplier = 0.5f;

    public Action<Vector2> OnRecoil;

    protected float _shootIntervalDuration;
    protected float _recoveryDuration;
    protected StopwatchTimer _timer;

    //StateMachine<State> _recoilFSM;

    /// <summary>
    /// �ݵ� �̺�Ʈ�� ȣ��� �� ����
    /// </summary>
    protected Action OnExecute;

    protected State _state;

    public enum State
    {
        Idle,
        Generate,
        Recover
    }

    public RecoilGenerator(float shootInterval, float recoveryDuration)
    {
        _shootIntervalDuration = shootInterval + 0.3f; // ���� Interval���� �� �� ũ�� ���ֱ�
        _recoveryDuration = recoveryDuration;
        _timer = new StopwatchTimer();

        _state = State.Idle;
    }

    public override void Execute() => GenerateRecoil();

    protected void StartMultiplying(Vector2 goalMultiplier, float duration)
    {
        _timer.Reset();
        _timer.Start(duration);
        _goalMultiplier = goalMultiplier;
    }

    protected void GenerateRecoil()
    {
        _state = State.Generate;
        Vector2 point = ReturnNextRecoilPoint();
        StartMultiplying(point, _shootIntervalDuration);
    }

    /// <summary>
    /// Recover�� ������ �� ȣ��
    /// </summary>
    protected virtual void OnStartRecovering() { }

    public override void OnUpdate()
    {
        switch (_state)
        {
            case State.Idle:
                return;
            case State.Generate:
                if (_timer.CurrentState != StopwatchTimer.State.Finish) break;

                _state = State.Recover;
                OnStartRecovering();
                StartMultiplying(Vector2.zero, _recoveryDuration);

                break;
            case State.Recover:
                if (_timer.CurrentState != StopwatchTimer.State.Finish) break;

                _state = State.Idle;
                _timer.Reset();
                break;
            default:
                break;
        }

        _viewRotationMultiplier = Vector2.Lerp(_viewRotationMultiplier, _goalMultiplier, _timer.Ratio); // ����Ǵ� ���� �Ѱ��ֱ�
        OnRecoil?.Invoke(_viewRotationMultiplier);
    }

    /// <summary>
    /// ���� �ݵ� ��ġ�� ��ȯ�Ѵ�.
    /// </summary>
    protected abstract Vector2 ReturnNextRecoilPoint();

    public override void LinkEvent(WeaponEventBlackboard blackboard)
    {
        OnRecoil += blackboard.OnRecoilRequested;
    }

    public override void UnlinkEvent(WeaponEventBlackboard blackboard)
    {
        OnRecoil -= blackboard.OnRecoilRequested;
    }


    /// <summary>
    /// �ٸ� ������ ������ ����� ���
    /// </summary>
    //public abstract void OnOtherActionEventRequested();


    /// <summary>
    /// Action �̺�Ʈ�� ȣ��Ǵ� Ÿ�ֿ̹� ����
    /// </summary>
    //public override void OnActionRequested() 
    //{ 

    //}

    /// <summary>
    /// Action �̺�Ʈ�� ����Ǵ� Ÿ�ֿ̹� ����
    /// </summary>
    //public override void OnActionFinishRequested() 
    //{ 

    //}



}
