using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomRecoilGenerator : AutoRecoilGenerator
{
    public ZoomRecoilGenerator(float shootInterval, float recoverTime, RecoilMap recoilMap) : base(shootInterval, recoverTime, recoilMap)
    {
        _actorBoneRecoilMultiplier = 1;
    }
}
