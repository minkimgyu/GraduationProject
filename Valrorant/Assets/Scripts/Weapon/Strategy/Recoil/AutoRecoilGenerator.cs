using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRecoilGenerator : RecoilGenerator
{
    int _index = -1;
    int _indexMultiplier = 1;
    int _repeatIndex = 0;

    float _recoilRatio;
    List<Vector2> _recoilPoints = new List<Vector2>();

    public AutoRecoilGenerator(float shootInterval, float recoveryDuration, float recoilRatio, RecoilMapData recoilData)
        : base(shootInterval, recoveryDuration)
    {
        _shootIntervalDuration = shootInterval;

        _recoilRatio = recoilRatio;
        _repeatIndex = recoilData.RepeatIndex;
        
        List<SerializableVector2> points = recoilData.ReturnAllAnglesBetweenCenterAndPoint();
        for (int i = 0; i < points.Count; i++) _recoilPoints.Add(points[i].V2);
    }

    protected override Vector2 ReturnNextRecoilPoint()
    {
        _index += _indexMultiplier;

        if (_index >= _recoilPoints.Count - 1) _indexMultiplier = -1; // 끝지점에 도착한 경우
        else if (_indexMultiplier == -1 && _index == _repeatIndex) _indexMultiplier = 1;
        // 반환 지점에서 _indexMultiplier가 -1인 경우

        return _recoilPoints[_index] * _recoilRatio;
    }

    protected override void OnStartRecovering() { _index = -1; _indexMultiplier = 1; } // 인덱스 초기화 필요
}
