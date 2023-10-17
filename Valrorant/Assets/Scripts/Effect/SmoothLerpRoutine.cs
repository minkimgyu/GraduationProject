using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SmoothLerpRoutine : MonoBehaviour
{
    [SerializeField]
    float predelay;

    [SerializeField]
    float duration;

    protected float smoothness = 0.001f;

    public Action<float> RoutineAction;
    public Action RoutineEnd;

    WaitForSeconds delayTime;
    WaitForSeconds predelayTime;

    Coroutine coroutine;

    public void Initialize()
    {
        delayTime = new WaitForSeconds(smoothness);
        predelayTime = new WaitForSeconds(predelay);
    }

    public void StopRoutine()
    {
        if(coroutine != null) StopCoroutine(coroutine);
    }

    public void StartRoutine()
    {
        coroutine = StartCoroutine(Routine());
    }

    public void StartRoutine(float duration)
    {
        coroutine = StartCoroutine(Routine(duration));
    }

    protected IEnumerator Routine(float duration)
    {
        yield return predelayTime;

        float progress = 0; //This float will serve as the 3rd parameter of the lerp function.
        float increment = smoothness / duration; //The amount of change to apply.
        while (progress < 1)
        {
            if (RoutineAction != null)
            {
                RoutineAction(progress);
            }

            progress += increment;
            yield return delayTime;
        }

        RoutineEnd();
    }

    protected IEnumerator Routine()
    {
        yield return predelayTime;

        float progress = 0; //This float will serve as the 3rd parameter of the lerp function.
        float increment = smoothness / duration; //The amount of change to apply.
        while (progress < 1)
        {
            if(RoutineAction != null)
            {
                RoutineAction(progress);
            }
            
            progress += increment;
            yield return delayTime;
        }

        RoutineEnd();
    }
}
