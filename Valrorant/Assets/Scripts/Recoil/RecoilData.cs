using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
abstract public class BaseRecoilData
{
    public BaseRecoilData(string name, float recoveryDuration, float distanceFromTarget, float ratioBetweenTargetAndDistance)
    {
        _name = name;
        _recoveryDuration = recoveryDuration;
        _distanceFromTarget = distanceFromTarget;
        _ratioBetweenTargetAndDistanceInPixel = ratioBetweenTargetAndDistance;
    }

    [SerializeField] string _name;
    public string Name { get { return _name; } }

    [SerializeField] float _recoveryDuration;
    public float RecoveryDuration { get { return _recoveryDuration; } }

    [SerializeField] protected float _distanceFromTarget;
    public float DistanceFromTarget { get { return _distanceFromTarget; } }

    [SerializeField] protected float _ratioBetweenTargetAndDistanceInPixel;
    public float RatioBetweenTargetAndDistanceInPixel { get { return _ratioBetweenTargetAndDistanceInPixel; } }

    protected Vector2 ReturnAngleBetweenCenterAndPoint(Vector2 point)
    {
        float toDegree = (float)(180.0f / Math.PI);

        float xAngle = (float)Math.Atan2(point.x * _ratioBetweenTargetAndDistanceInPixel, _distanceFromTarget) * toDegree;
        float yAngle = (float)Math.Atan2(point.y * _ratioBetweenTargetAndDistanceInPixel, _distanceFromTarget) * toDegree;

        return new Vector2(xAngle, yAngle);
    }
}

[Serializable]
public class RecoilMapData : BaseRecoilData
{
    public RecoilMapData(string name, float recoveryDuration, float distanceFromTarget, float ratioBetweenTargetAndDistance, int selectedIndex, int repeatIndex, List<Vector2> points)
        : base(name, recoveryDuration, distanceFromTarget, ratioBetweenTargetAndDistance)
    {
        _selectedIndex = selectedIndex;
        _repeatIndex = repeatIndex;
        _points = points;
    }

    [SerializeField] int _selectedIndex;
    public int SelectedIndex { get { return _selectedIndex; } }

    [SerializeField] int _repeatIndex;
    public int RepeatIndex { get { return _repeatIndex; } }

    [SerializeField] List<Vector2> _points;
    public List<Vector2> Points { get { return _points; } } // --> point¿”

    public List<Vector2> ReturnAllAnglesBetweenCenterAndPoint()
    {
        List<Vector2> tmpList = new List<Vector2>();
        for (int i = 0; i < _points.Count; i++)
        {
            tmpList.Add(ReturnAngleBetweenCenterAndPoint(_points[i]));
        }

        return tmpList;
    }
}

[Serializable]
public class RecoilRangeData : BaseRecoilData
{
    public RecoilRangeData(string name, float recoveryDuration, float distanceFromTarget, float ratioBetweenTargetAndDistance, Vector2 point)
        : base(name, recoveryDuration, distanceFromTarget, ratioBetweenTargetAndDistance)
    {
        _point = point;
    }

    [SerializeField] Vector2 _point;
    public Vector2 Point { get { return _point; } }

    public Vector2 ReturnFixedPoint()
    {
        return ReturnAngleBetweenCenterAndPoint(_point);
    }
}
