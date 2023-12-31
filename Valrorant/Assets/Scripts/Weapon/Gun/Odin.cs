using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

[System.Serializable]
public class Odin : AutomaticGun
{
    public override void Initialize(GameObject player, GameObject armMesh, Transform cam, Animator ownerAnimator)
    {
        _damageDictionary = new Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]>()
        {
            { DistanceAreaData.HitArea.Head, new DistanceAreaData[]{ new DistanceAreaData(0, 30, 95), new DistanceAreaData(30, 50, 77) } },
            { DistanceAreaData.HitArea.Body, new DistanceAreaData[]{ new DistanceAreaData(0, 30, 38), new DistanceAreaData(30, 50, 31) } },
            { DistanceAreaData.HitArea.Leg, new DistanceAreaData[]{ new DistanceAreaData(0, 30, 32), new DistanceAreaData(30, 50, 26) } },
        };

        base.Initialize(player, armMesh, cam, ownerAnimator);
    }
}
