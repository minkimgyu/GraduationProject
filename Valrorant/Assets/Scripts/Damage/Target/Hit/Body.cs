using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

public class Body : SeparateHitTarget
{
    protected override void Start()
    {
        base.Start();
        _durability = 3;
        hitArea = DistanceAreaData.HitArea.Body;
    }
}
