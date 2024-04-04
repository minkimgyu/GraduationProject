using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

public class Head : SeparateHitTarget
{
    protected override void Start()
    {
        base.Start();
        hitArea = DistanceAreaData.HitArea.Head;
    }
}
