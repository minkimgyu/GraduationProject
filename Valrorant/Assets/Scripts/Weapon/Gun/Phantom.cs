using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

[System.Serializable]
public class Phantom : AutomaticGun
{
    public override void Initialize(GameObject player, GameObject armMesh, Transform cam, Animator ownerAnimator)
    {
        _damageDictionary = new Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]>()
        {
            { DistanceAreaData.HitArea.Head, new DistanceAreaData[]{ new DistanceAreaData(0, 15, 156), new DistanceAreaData(15, 30, 140), new DistanceAreaData(30, 50, 124) } },
            { DistanceAreaData.HitArea.Body, new DistanceAreaData[]{ new DistanceAreaData(0, 15, 39), new DistanceAreaData(15, 30, 35), new DistanceAreaData(30, 50, 31) } },
            { DistanceAreaData.HitArea.Leg, new DistanceAreaData[]{ new DistanceAreaData(0, 15, 33), new DistanceAreaData(15, 30, 29), new DistanceAreaData(30, 50, 26) } },
        };

        base.Initialize(player, armMesh, cam, ownerAnimator);
    }
}
