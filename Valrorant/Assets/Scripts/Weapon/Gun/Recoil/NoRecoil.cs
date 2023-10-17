using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoRecoil : RecoilStrategy
{
    public NoRecoil(float shootInterval = 0, float recoverTime = 0) : base(shootInterval, recoverTime)
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
