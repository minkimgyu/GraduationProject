using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualRecoilGenerator : RecoilGenerator
{
    Vector2 _recoilDirection;

    public ManualRecoilGenerator(float shootInterval, float recoveryDuration, RecoilRangeData recoilRange)
        : base(shootInterval, recoveryDuration)
    {
        _shootIntervalDuration = shootInterval;
        _recoilDirection = recoilRange.ReturnFixedPoint().V2;
    }

    protected override Vector2 ReturnNextRecoilPoint() { return _recoilDirection; }
}
