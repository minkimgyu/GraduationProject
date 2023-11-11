using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public interface IRoutine
{
    Transform ReturnTransform();

    GameObject ReturnGameObject();

    T GetComponentInRoutineClass<T>();

    T GetComponentInChildrenInRoutineClass<T>();
}

public interface IWeaponRoutine : IRoutine
{
    Action<Collision> OnCollisionEnterRequested { get; set; }
}

public interface IEffectRoutine : IRoutine
{
    Action OnUpdate { get; set; }
    Action OnDisableGameObject { get; set; }
}

public interface IInteractContainer
{
    IInteractable ReturnInteractableObject();
}

public interface IWeaponContainer
{
    BaseWeapon ReturnWeapon();
}

public interface IEffectContainer
{
    BaseEffect ReturnEffect();
}

public class AbstractContainer<W> : MonoBehaviour
{
    [SerializeField]
    protected W _storedRoutine;

    public W StoredRoutine { get { return _storedRoutine; } }

    private void Awake() => SetUp();

    protected virtual void SetUp() { }
}

public class WeaponContainer<W> : AbstractContainer<W>, IWeaponRoutine
{
    public Action<Collision> OnCollisionEnterRequested { get; set; }

    private void OnCollisionEnter(Collision collision) => OnCollisionEnterRequested(collision);

    public T GetComponentInRoutineClass<T>()
    {
        return GetComponent<T>();
    }

    public T GetComponentInChildrenInRoutineClass<T>()
    {
        return GetComponentInChildren<T>();
    }

    public GameObject ReturnGameObject() { return gameObject; }

    public Transform ReturnTransform() { return transform; }
}

public class EffectContainer<W> : AbstractContainer<W>, IEffectRoutine
{
    public Action OnUpdate { get; set; }
    public Action OnDisableGameObject { get; set; }

    private void Update() => OnUpdate();

    private void OnDisable() => OnDisableGameObject();

    public T GetComponentInRoutineClass<T>()
    {
        return GetComponent<T>();
    }

    public T GetComponentInChildrenInRoutineClass<T>()
    {
        return GetComponentInChildren<T>();
    }

    public GameObject ReturnGameObject() { return gameObject; }

    public Transform ReturnTransform() { return transform; }
}