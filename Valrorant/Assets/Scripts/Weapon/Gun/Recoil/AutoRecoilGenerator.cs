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

    public override void RecoverRecoil() // _viewRotationMultiplier �̰Ÿ� 0���� ������ָ� ��
    {
        StopLerp();

        _indexMultiplier = 1;
        ResetIndex();
        StartLerp(Vector2.zero, _recoverDuration);
    }

    protected override Vector2 ReturnNextRecoilPoint()
    {
        _index += _indexMultiplier;

        if (_index >= _recoilMap.RecoilData.Length - 1) // �������� ������ ���
        {
            _indexMultiplier = -1;
        }
        else if (_indexMultiplier == -1 && _index == _recoilMap.RepeatIndex) // ��ȯ �������� _indexMultiplier�� -1�� ���
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
