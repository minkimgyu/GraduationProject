using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

public class Body : SeparateHitTarget
{
    protected override void Start()
    {
        base.Start();
        hitArea = DistanceAreaData.HitArea.Body;
    }
}
