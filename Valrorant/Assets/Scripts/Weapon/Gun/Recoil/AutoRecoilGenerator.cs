using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRecoilGenerator : RecoilStrategy
{
    int _index = -1;
    int _indexMultiplier = 1;

    RecoilMap _recoilMap;

    public AutoRecoilGenerator(float shootInterval, float recoverTime, RecoilMap recoilMap) : base(shootInterval, recoverTime)
    {
        _recoilMap = recoilMap;
    }

    public override void CreateRecoil()
    {
        StopLerp();

        Vector2 point = ReturnNextRecoilPoint();
        StartLerp(point, _shootInterval);
    }

    public override void RecoverRecoil() // _viewRotationMultiplier 이거를 0으로 만들어주면 됨
    {
        StopLerp();

        _indexMultiplier = 1;
        ResetIndex();
        StartLerp(Vector2.zero, _recoverDuration);
    }

    protected override Vector2 ReturnNextRecoilPoint()
    {
        _index += _indexMultiplier;

        if (_index >= _recoilMap.RecoilData.Length - 1) // 끝지점에 도착한 경우
        {
            _indexMultiplier = -1;
        }
        else if (_indexMultiplier == -1 && _index == _recoilMap.RepeatIndex) // 반환 지점에서 _indexMultiplier가 -1인 경우
        {
            _indexMultiplier = 1;
        }

        return _recoilMap.RecoilData[_index];
    }


    void ResetIndex()
    {
        _index = -1;
    }
}
