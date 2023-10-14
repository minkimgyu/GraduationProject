using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    bool isFinish = false;

    bool startTimer = false;
    float _storedDelayTime = 0;
    float _delayTime = 0;


    public void Start(float delayTime)
    {
        _delayTime = delayTime;
        startTimer = true;
        isFinish = false;
    }

    public void Update()
    {
        if (startTimer == false) return;

        _storedDelayTime += Time.deltaTime;
        if (_storedDelayTime >= _delayTime)
        {
            _storedDelayTime = 0;
            startTimer = false;
            isFinish = true;
        }
    }

    public bool IsTimerFinish()
    {
        if(isFinish == true)
        {
            isFinish = false;
            return true;
        }

        return false;
    }
}
