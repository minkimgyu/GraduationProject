using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SmoothLerpUtility : MonoBehaviour
{
    Timer _timer;
    public Action<float> OnLerpRequested;

    public SmoothLerpUtility()
    {
        _timer = new Timer();
    }

    public bool IsFinish()
    {
        return _timer.IsFinish;
    }

    private void Update()
    {
        _timer.Update();
        OnLerpRequested(_timer.Ratio);
    }

    public void StartSmoothLerp(float duration)
    {
        _timer.Start(duration);
    }

    public void StopSmoothLerp()
    {
        _timer.Reset();
    }
}
