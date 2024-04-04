using System.Collections;
using UnityEngine;

//abstract public class BaseRoutine<T>
//{
//    protected IRoutine _iRoutine;

//    public GameObject gameObject { get { return _iRoutine.ReturnGameObject(); } }
//    public Transform transform { get { return _iRoutine.ReturnTransform(); } }

//    public W GetComponent<W>()
//    {
//        return _iRoutine.GetComponentInRoutineClass<W>();
//    }

//    public W GetComponentInChildren<W>()
//    {
//        return _iRoutine.GetComponentInChildrenInRoutineClass<W>();
//    }

//    public GameObject FindWithTag(string tag)
//    {
//        return GameObject.FindWithTag(tag);
//    }

//    public abstract void SetUp(T routine);
//}

//abstract public class WeaponRoutine : BaseRoutine<IWeaponRoutine>
//{
//    protected abstract void OnCollisionEnterRequested(Collision collision);

//    public override void SetUp(IWeaponRoutine iWeaponRoutine)
//    {
//        iWeaponRoutine.OnCollisionEnterRequested = OnCollisionEnterRequested;
//        _iRoutine = iWeaponRoutine;
//    }
//}

//abstract public class EffectRoutine : BaseRoutine<IEffectRoutine>
//{
//    protected abstract void OnUpdate();

//    protected abstract void OnDisableGameObject();

//    public override void SetUp(IEffectRoutine iEffectRoutine)
//    {
//        iEffectRoutine.OnUpdate = OnUpdate;
//        iEffectRoutine.OnDisableGameObject = OnDisableGameObject;
//        _iRoutine = iEffectRoutine;
//    }
//}