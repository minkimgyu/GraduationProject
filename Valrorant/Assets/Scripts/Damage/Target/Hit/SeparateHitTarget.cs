using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

public class SeparateHitTarget : BaseHitTarget
{
    protected override void Start()
    {
        base.Start();
        IDamage = GetComponentInParent<IDamageable>();
    }
}
