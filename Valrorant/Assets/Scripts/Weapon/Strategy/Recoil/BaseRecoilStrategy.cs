using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

abstract public class BaseRecoilStrategy : BaseStrategy
{
    public BaseRecoilStrategy() { }

    /// <summary>
    /// Action 이벤트가 호출되는 타이밍에 실행
    /// </summary>
    public virtual void Execute() { }
}

abstract public class RecoilGenerator : BaseRecoilStrategy
{
    protected Vector2 _viewRotationMultiplier;
    protected Vector2 _goalMultiplier;

    public Action<Vector2> OnRecoil;

    protected float _shootIntervalDuration;
    protected float _recoveryDuration;
    protected StopwatchTimer _timer;

    /// <summary>
    /// 반동 이벤트가 호출될 때 실행
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
        _shootIntervalDuration = shootInterval + 0.3f; // 실제 Interval보다 좀 더 크게 해주기
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
    /// Recover를 시작할 때 호출
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

        _viewRotationMultiplier = Vector2.Lerp(_viewRotationMultiplier, _goalMultiplier, _timer.Ratio); // 변경되는 값을 넘겨주기
        OnRecoil?.Invoke(_viewRotationMultiplier);
    }

    /// <summary>
    /// 다음 반동 위치를 반환한다.
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
}
