using System.Collections;
using UnityEngine;

abstract public class BaseRoutine
{
    IWeaponContainer _iContainer;

    public GameObject gameObject { get { return _iContainer.ReturnGameObject(); } }
    public Transform transform { get { return _iContainer.ReturnTransform(); } }

    public Coroutine StartCoroutine(IEnumerator enumerator)
    {
        return _iContainer.StartCoroutineInRoutineClass(enumerator);
    }

    public void StopCoroutine(IEnumerator enumerator)
    {
        _iContainer.StopCoroutineInRoutineClass(enumerator);
    }

    public void StopCoroutine(Coroutine routine)
    {
        _iContainer.StopCoroutineInRoutineClass(routine);
    }

    public T GetComponent<T>()
    {
        return _iContainer.GetComponentInRoutineClass<T>();
    }

    public GameObject FindWithTag(string tag)
    {
        return GameObject.FindWithTag(tag);
    }

    protected abstract void OnUpdate();

    public void SetUp(IWeaponContainer iHoldWeapon)
    {
        _iContainer = iHoldWeapon;
        _iContainer.OnUpdate = OnUpdate;
    }
}
