using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPenetrable
{
    public enum HitEffectType
    {
        BulletPenetration,
        BulletNonPenetration,
        KnifeAttack
    }

    public float ReturnDurability();

    public GameObject ReturnPenetrableTarget();

    public bool CanReturnHitEffectName(HitEffectType effectType, out string effectName);
}
