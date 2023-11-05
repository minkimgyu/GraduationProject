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

    Action OnUpdate { get; set; }
}


public interface IWeaponContainer : IRoutine
{
    BaseWeapon ReturnWeapon();
}

public class AbstractContainer<W> : MonoBehaviour, IWeaponContainer
{
    [SerializeField]
    protected W _storedRoutine;

    public W StoredRoutine { get { return _storedRoutine; } }

    public Action OnUpdate { get; set; }

    private void Awake() => SetUp();

    private void Update() => OnUpdate();

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

    public virtual BaseWeapon ReturnWeapon() { return null; }

    public GameObject ReturnGameObject() { return gameObject;  }

    public Transform ReturnTransform() { return transform; }
}