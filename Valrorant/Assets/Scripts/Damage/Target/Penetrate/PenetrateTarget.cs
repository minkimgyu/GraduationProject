using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenetrateTarget : MonoBehaviour, IPenetrable, IEffectable
{
    [SerializeField]
    float durability;

    [SerializeField]
    string bulletPenetrationEffectName;

    [SerializeField]
    string BulletNonPenetrationEffectName;

    [SerializeField]
    string knifeAttackEffectName;

    protected Dictionary<IEffectable.ConditionType, string> hitEffect;

    // Start is called before the first frame update
    void Start()
    {
        hitEffect = new Dictionary<IEffectable.ConditionType, string>()
        {
            {IEffectable.ConditionType.BulletPenetration, bulletPenetrationEffectName },
            {IEffectable.ConditionType.BulletNonPenetration, BulletNonPenetrationEffectName },
            {IEffectable.ConditionType.KnifeAttack, knifeAttackEffectName }
        };
    }

    public float ReturnDurability()
    {
        return durability;
    }

    public bool CanReturnHitEffectName(IEffectable.ConditionType effectType)
    {
        if (hitEffect.ContainsKey(effectType) == false) return false;

        return true;
    }

    public string ReturnHitEffectName(IEffectable.ConditionType effectType)
    {
        return hitEffect[effectType];
    }

    public GameObject ReturnAttachedObject()
    {
        return gameObject;
    }
}
