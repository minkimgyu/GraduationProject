using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPenetrable : IAttachedObject
{
    public float ReturnDurability();
}

public interface IAttachedObject
{
    public GameObject ReturnAttachedObject();
}

public interface IEffectable
{
    public enum ConditionType
    {
        BulletPenetration,
        BulletNonPenetration,
        KnifeAttack
    }

    public bool CanReturnHitEffectName(ConditionType effectType);

    public string ReturnHitEffectName(ConditionType effectType);
}