using System.Collections;
using UnityEngine;

abstract public class BaseAbstractRoutineClass
{
    AbstractRoutineContainerClass _abstractContainerClass;

    public GameObject GameObject { get { return _abstractContainerClass.gameObject; } }

    public Coroutine StartCoroutine(IEnumerator enumerator)
    {
        return _abstractContainerClass.StartCoroutineInRoutineClass(enumerator);
    }

    public void StopCoroutine(IEnumerator enumerator)
    {
        _abstractContainerClass.StopCoroutineInRoutineClass(enumerator);
    }

    public void StopCoroutine(Coroutine routine)
    {
        _abstractContainerClass.StopCoroutineInRoutineClass(routine);
    }

    public T GetComponent<T>()
    {
        return _abstractContainerClass.GetComponentInRoutineClass<T>();
    }

    public void SetUp(AbstractRoutineContainerClass abstractContainerClass)
    {
        _abstractContainerClass = abstractContainerClass;

        _abstractContainerClass.OnAwakeCall = OnAwake;
        _abstractContainerClass.OnStartCall = OnStart;
        _abstractContainerClass.OnUpdateCall = OnUpdate;
        _abstractContainerClass.OnFixedUpdateCall = OnFixedUpdate;
        _abstractContainerClass.OnLateUpdateCall = OnLateUpdate;
    }
    public abstract void Initialize(GameObject player, Transform cam, Animator ownerAnimator);

    protected abstract void OnAwake();
    protected abstract void OnStart();

    protected abstract void OnUpdate();
    protected abstract void OnFixedUpdate();
    protected abstract void OnLateUpdate();
}
