using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

abstract public class BaseCaptureComponent<T> : MonoBehaviour where T : ITarget
{
    [SerializeField] protected float _captureRadius;

    [SerializeField] SphereCollider _captureCollider;
    [SerializeField] BaseDrawer _sphereDrawer;
    [SerializeField] protected LayerMask _layerMask;
    [SerializeField] TargetType _targetType;

    public virtual void Initialize(float radius, int maxQueueSize, Action OnNoiseReceived) { }
    public virtual void Initialize(float radius, float angle) { }
    public virtual void Initialize(float radius) { }

    public void ModifyCaptureRadius(float radius)
    {
        _captureRadius += radius;
        OnModifyData();
    }

    protected virtual void OnTargetEnter(T target) { }

    protected virtual void OnTargetExit(T target) { }

    protected abstract bool IsAlreadyContaining(T target);

    protected bool IsRightLayer(Collider other)
    {
        int value = (_layerMask & (1 << other.gameObject.layer));
        return value != 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "Paladin")
        {
            Debug.Log("Paladin");
        }

        if (IsRightLayer(other) == false) return;

        T component =  other.GetComponent<T>();
        if (component == null || component.MyType != _targetType) return;

        if (IsAlreadyContaining(component) == true) return;

        OnTargetEnter(component);
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsRightLayer(other) == false) return;

        T component = other.GetComponent<T>();
        if (component == null || component.MyType != _targetType) return;

        if (IsAlreadyContaining(component) == false) return;

        OnTargetExit(component);
    }

    protected virtual void OnModifyData()
    {
        if (_sphereDrawer == null) return;
        _sphereDrawer.ResetData(_captureRadius);

        if (_captureCollider == null) return;
        _captureCollider.radius = _captureRadius;
    }

    protected virtual void OnValidate()
    {
        OnModifyData();
    }
}
