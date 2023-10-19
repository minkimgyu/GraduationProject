using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Timer
{
    bool _isRunning = false;
    public bool IsFinish { get { return !_isRunning; } }
    public bool IsRunning { get { return _isRunning; } }

    float _currentTime = 0;
    float _duration = 0;

    public float Ratio { get { return _currentTime / _duration; } }

    public void Start(float duration)
    {
        _duration = duration;
        _isRunning = true;
    }

    public void Stop()
    {
        _isRunning = false;
        _currentTime = 0;
    }

    public void Update()
    {
        if (_isRunning == false) return;

        _currentTime += Time.smoothDeltaTime;
        if (_currentTime >= _duration)
        {
            Stop();
        }
    }
}
