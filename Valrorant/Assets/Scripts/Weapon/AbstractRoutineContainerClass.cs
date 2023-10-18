using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AbstractRoutineContainerClass: MonoBehaviour
{
    protected BaseAbstractRoutineClass _baseAbstractRoutineClass;
    public BaseAbstractRoutineClass BaseAbstractRoutineClass { get { return _baseAbstractRoutineClass; } }

    public Action OnAwakeCall;
    public Action OnStartCall;
    public Action OnUpdateCall;
    public Action OnFixedUpdateCall;
    public Action OnLateUpdateCall;

    public Coroutine StartCoroutineInRoutineClass(IEnumerator enumerator)
    {
        return StartCoroutine(enumerator);
    }

    public void StopCoroutineInRoutineClass(IEnumerator enumerator)
    {
        StopCoroutine(enumerator);
    }

    public void StopCoroutineInRoutineClass(Coroutine routine)
    {
        StopCoroutine(routine);
    }

    public T GetComponentInRoutineClass<T>()
    {
        return GetComponent<T>();
    }

    protected virtual void Awake()
    {
        _baseAbstractRoutineClass.SetUp(this);

        if (OnAwakeCall != null) OnAwakeCall();
    }

    private void Start()
    {
        if (OnStartCall != null) OnStartCall();
    }

    private void Update()
    {
        if (OnUpdateCall != null) OnUpdateCall();
    }

    private void FixedUpdate()
    {
        if (OnFixedUpdateCall != null) OnFixedUpdateCall();
    }

    private void LateUpdate()
    {
        if (OnLateUpdateCall != null) OnLateUpdateCall();
    }
}