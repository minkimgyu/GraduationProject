using DamageUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectDamageTarget : BaseHitTarget
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        IDamage = GetComponent<IDamageable>();
    }
}
