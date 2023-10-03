using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PenetrateData
{
    const float _airDurability = 3;
    public float AirDurability { get { return _airDurability; } }

    Vector3 _entryPoint;
    public Vector3 EntryPoint { get { return _entryPoint; } }

    Vector3 _entryNormal;
    public Vector3 EntryNormal { get { return _entryNormal; } }


    Vector3 _exitPoint;
    public Vector3 ExitPoint { get { return _exitPoint; } }

    Vector3 _exitNormal;
    public Vector3 ExitNormal { get { return _exitNormal; } }


    IPenetrateTarget _target;

    public float ReturnFinalDurability()
    {
        return ReturnDistance() * _target.ReturnDurability(); // 거리와 내구성 곱연산
    }

    public float ReturnDistance()
    {
        return Vector3.Distance(_entryPoint, _exitPoint);
    }

    public PenetrateData(Vector3 entryPoint, Vector3 exitPoint, Vector3 entryNormal, Vector3 exitNormal, IPenetrateTarget target)
    {
        _entryPoint = entryPoint;
        _exitPoint = exitPoint;

        _entryNormal = entryNormal;
        _exitNormal = exitNormal;

        _target = target;
    }
}
