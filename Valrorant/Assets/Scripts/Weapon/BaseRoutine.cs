using System.Collections;
using UnityEngine;

abstract public class BaseRoutine
{
    IRoutine _iRoutine;

    public GameObject gameObject { get { return _iRoutine.ReturnGameObject(); } }
    public Transform transform { get { return _iRoutine.ReturnTransform(); } }

    public Coroutine StartCoroutine(IEnumerator enumerator)
    {
        return _iRoutine.StartCoroutineInRoutineClass(enumerator);
    }

    public T GetComponent<T>()
    {
        return _iRoutine.GetComponentInRoutineClass<T>();
    }

    public T GetComponentInChildren<T>()
    {
        return _iRoutine.GetComponentInChildrenInRoutineClass<T>();
    }

    public GameObject FindWithTag(string tag)
    {
        return GameObject.FindWithTag(tag);
    }

    protected abstract void OnUpdate();

    protected abstract void OnDisableGameObject();

    public void SetUp(IRoutine iRoutine)
    {
        _iRoutine = iRoutine;
        _iRoutine.OnUpdate = OnUpdate;
        _iRoutine.OnDisableGameObject = OnDisableGameObject;
    }
}
