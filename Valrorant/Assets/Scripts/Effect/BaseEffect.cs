using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class BaseEffect : MonoBehaviour
{
    [SerializeField]
    protected float _duration;

    protected Timer _timer = new Timer();

    public virtual void Initialize(Vector3 hitPosition) { }

    public virtual void Initialize(Vector3 hitPosition, Vector3 shootPosition) { }

    public virtual void Initialize(Vector3 hitPosition, Vector3 hitNormal, Quaternion holeRotation) { }

    public virtual void Initialize(Vector3 hitPosition, Vector3 hitNormal, Quaternion holeRotation, float damamge) { }

    abstract public void PlayEffect();

    protected virtual void OnDisable()
    {
        _timer.Reset();
        ObjectPooler.ReturnToPool(gameObject);
    }

    protected void DisableObject() => gameObject.SetActive(false);
}