using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

[Serializable]
public struct SerializableVector2
{
    public SerializableVector2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public float x;
    public float y;

    [JsonIgnore]
    public Vector3 V2 { get { return new Vector3(x, y); } }
}

[Serializable]
abstract public class BaseRecoilData
{
    public BaseRecoilData(string name, float distanceFromTarget, float ratioBetweenTargetAndDistance)
    {
        _name = name;
        _distanceFromTarget = distanceFromTarget;
        _ratioBetweenTargetAndDistanceInPixel = ratioBetweenTargetAndDistance;
    }

    public string _name;
    [JsonIgnore] public string Name { get { return _name; } }

    public float _distanceFromTarget;
    [JsonIgnore] public float DistanceFromTarget { get { return _distanceFromTarget; } }

    public float _ratioBetweenTargetAndDistanceInPixel;
    [JsonIgnore] public float RatioBetweenTargetAndDistanceInPixel { get { return _ratioBetweenTargetAndDistanceInPixel; } }

    protected SerializableVector2 ReturnAngleBetweenCenterAndPoint(SerializableVector2 point)
    {
        float toDegree = (float)(180.0f / Math.PI);

        float xAngle = (float)Math.Atan2(point.x * _ratioBetweenTargetAndDistanceInPixel, _distanceFromTarget) * toDegree;
        float yAngle = (float)Math.Atan2(point.y * _ratioBetweenTargetAndDistanceInPixel, _distanceFromTarget) * toDegree;

        return new SerializableVector2(xAngle, yAngle);
    }
}

[Serializable]
public class RecoilMapData : BaseRecoilData
{
    public RecoilMapData(string name, float distanceFromTarget, float ratioBetweenTargetAndDistance, int selectedIndex, int repeatIndex, List<SerializableVector2> points)
        : base(name, distanceFromTarget, ratioBetweenTargetAndDistance)
    {
        _selectedIndex = selectedIndex;
        _repeatIndex = repeatIndex;
        _points = points;
    }

    public int _selectedIndex;
    [JsonIgnore] public int SelectedIndex { get { return _selectedIndex; } }

    public int _repeatIndex;
    [JsonIgnore] public int RepeatIndex { get { return _repeatIndex; } }

    public List<SerializableVector2> _points;
    [JsonIgnore] public List<SerializableVector2> Points { get { return _points; } } // --> point¿”

    public List<SerializableVector2> ReturnAllAnglesBetweenCenterAndPoint()
    {
        List<SerializableVector2> tmpList = new List<SerializableVector2>();
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
    public RecoilRangeData(string name, float distanceFromTarget, float ratioBetweenTargetAndDistance, SerializableVector2 point)
        : base(name, distanceFromTarget, ratioBetweenTargetAndDistance)
    {
        _point = point;
    }

    public SerializableVector2 _point;
    [JsonIgnore] public SerializableVector2 Point { get { return _point; } }

    public SerializableVector2 ReturnFixedPoint()
    {
        return ReturnAngleBetweenCenterAndPoint(_point);
    }
}
