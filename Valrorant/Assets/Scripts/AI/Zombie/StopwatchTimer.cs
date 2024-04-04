using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class StopwatchTimer
{
    public enum State
    {
        Ready,
        Running,
        Finish
    }

    State _state;
    float _startTime, _duration;

    public State CurrentState
    {
        get
        {
            if (_state == State.Ready) return _state;
            else
            {
                if(_duration <= Time.time - _startTime) _state = State.Finish;
                return _state;
            }
        }
    }

    public StopwatchTimer()
    {
        _state = State.Ready;
        _startTime = 0;
        _duration = 0;
    }

    public void Start(float duration)
    {
        if (_state != State.Ready) return;
        _state = State.Running;

        _startTime = Time.time;
        _duration = duration;
    }

    // 타이머를 처음으로 초기화해준다.
    public void Reset()
    {
        if (_state == State.Ready) return;

        _state = State.Ready;
        _startTime = 0;
        _duration = 0;
    }
}