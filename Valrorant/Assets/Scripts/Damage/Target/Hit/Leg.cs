using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

public class Leg : SeparateHitTarget
{
    protected override void Start()
    {
        base.Start();
        hitArea = DistanceAreaData.HitArea.Leg;
    }
}
