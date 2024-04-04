using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTarget : MonoBehaviour, ITarget
{
    public TargetType MyType { get; set; }

    private void Start()
    {
        MyType = TargetType.Human;
    }

    public Vector3 ReturnPos()
    {
        return transform.position;
    }

    public Transform ReturnTransform()
    {
        return transform;
    }
}
