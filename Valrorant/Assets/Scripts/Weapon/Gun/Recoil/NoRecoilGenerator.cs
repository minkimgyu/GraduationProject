using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoRecoilGenerator : RecoilStrategy
{
    public NoRecoilGenerator(float shootInterval = 0, float recoverTime = 0) : base(shootInterval, recoverTime)
    {
    }

    public override void CreateRecoil()
    {
    }

    public override void RecoverRecoil()
    {
    }

    protected override Vector2 ReturnNextRecoilPoint()
    {
        return default;
    }
}
