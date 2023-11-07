using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class BaseEffect : BaseRoutine
{
    [SerializeField]
    protected float _duration;

    protected Timer _timer = new Timer();

    public virtual void Initialize(Vector3 hitPosition) { }

    public virtual void Initialize(Vector3 hitPosition, Vector3 shootPosition) { }

    public virtual void Initialize(Vector3 hitPosition, Vector3 hitNormal, Quaternion holeRotation) { }

    public virtual void Initialize(Vector3 hitPosition, Vector3 hitNormal, Quaternion holeRotation, float damamge) { }

    abstract public void PlayEffect();

    protected override void OnDisableGameObject()
    {
        _timer.Reset();
        ObjectPooler.ReturnToPool(gameObject);
    }

    protected void DisableObject() => gameObject.SetActive(false);
}