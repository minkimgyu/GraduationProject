using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTarget : MonoBehaviour, ITarget
{
    [SerializeField] Transform _sightPoint;
    [SerializeField] TargetType type;
    public TargetType MyType { get; set; }

    private void Start()
    {
        MyType = type;
    }

    public Vector3 ReturnPos()
    {
        return transform.position;
    }

    public Transform ReturnTransform()
    {
        return transform;
    }

    public Transform ReturnSightPoint()
    {
        return _sightPoint;
    }
}
