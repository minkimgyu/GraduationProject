using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

[System.Serializable]
public class Vandal : AutomaticGun
{
    public override void Initialize(GameObject player, GameObject armMesh, Transform cam, Animator ownerAnimator)
    {
        _damageDictionary = new Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]>()
        {
            { DistanceAreaData.HitArea.Head, new DistanceAreaData[]{ new DistanceAreaData(0, 50, 160) } },
            { DistanceAreaData.HitArea.Body, new DistanceAreaData[]{ new DistanceAreaData(0, 50, 40) } },
            { DistanceAreaData.HitArea.Leg, new DistanceAreaData[]{ new DistanceAreaData(0, 50, 34) } },
        };

        base.Initialize(player, armMesh, cam, ownerAnimator);
    }
}
