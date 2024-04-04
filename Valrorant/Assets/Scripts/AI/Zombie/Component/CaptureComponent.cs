using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureComponent<T> : MonoBehaviour
{
    [SerializeField] protected List<T> _components;
    protected string _captureTag = "";

    public virtual void Initialize(float captureRadius) 
    {
        _components = new List<T>();
    }

    public virtual void Initialize(string captureTag)
    {
        _components = new List<T>();
    }

    private void OnTriggerEnter(Collider other)
    {
        T component = other.GetComponent<T>();
        if (component == null) return;

        if(_captureTag == "" || (_captureTag != "" && other.CompareTag(_captureTag) == true))
            _components.Add(component);
    }

    private void OnTriggerExit(Collider other)
    {
        T component = other.GetComponent<T>();
        if (component == null) return;

        if (_captureTag == "" || (_captureTag != "" && other.CompareTag(_captureTag) == true))
            _components.Remove(component);
    }
    public List<T> ReturnNearComponents() { return _components; }
}
