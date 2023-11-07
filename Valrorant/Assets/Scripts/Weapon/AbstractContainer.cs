using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public interface IRoutine
{
    Transform ReturnTransform();
    GameObject ReturnGameObject();

    Coroutine StartCoroutineInRoutineClass(IEnumerator enumerator);

    void StopCoroutineInRoutineClass(IEnumerator enumerator);

    void StopCoroutineInRoutineClass(Coroutine routine);

    T GetComponentInRoutineClass<T>();

    T GetComponentInChildrenInRoutineClass<T>();

    Action OnUpdate { get; set; }
    Action OnDisableGameObject { get; set; }
}


public interface IWeaponContainer
{
    BaseWeapon ReturnWeapon();
}

public interface IEffectContainer
{
    BaseEffect ReturnEffect();
}

public class AbstractContainer<W> : MonoBehaviour, IRoutine
{
    [SerializeField]
    protected W _storedRoutine;

    public W StoredRoutine { get { return _storedRoutine; } }

    public Action OnUpdate { get; set; }
    public Action OnDisableGameObject { get; set; }

    private void Awake() => SetUp();

    private void Update() => OnUpdate();

    private void OnDisable() => OnDisableGameObject();

    protected virtual void SetUp() { }

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

    public T GetComponentInChildrenInRoutineClass<T>()
    {
        return GetComponentInChildren<T>();
    }

    public GameObject ReturnGameObject() { return gameObject;  }

    public Transform ReturnTransform() { return transform; }
}