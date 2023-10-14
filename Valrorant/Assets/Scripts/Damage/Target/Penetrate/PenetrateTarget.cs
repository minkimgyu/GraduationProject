using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenetrateTarget : MonoBehaviour, IPenetrable
{
    [SerializeField]
    float durability;

    [SerializeField]
    string bulletPenetrationEffectName;

    [SerializeField]
    string BulletNonPenetrationEffectName;

    [SerializeField]
    string knifeAttackEffectName;

    protected Dictionary<IPenetrable.HitEffectType, string> hitEffect;

    // Start is called before the first frame update
    void Start()
    {
        hitEffect = new Dictionary<IPenetrable.HitEffectType, string>()
        {
            {IPenetrable.HitEffectType.BulletPenetration, bulletPenetrationEffectName },
            {IPenetrable.HitEffectType.BulletNonPenetration, BulletNonPenetrationEffectName },
            {IPenetrable.HitEffectType.KnifeAttack, knifeAttackEffectName }
        };
    }

    public float ReturnDurability()
    {
        return durability;
    }

    public GameObject ReturnPenetrableTarget()
    {
        return gameObject;
    }

    public bool CanReturnHitEffectName(IPenetrable.HitEffectType effectType, out string effectName)
    {
        effectName = "";
        if (hitEffect.ContainsKey(effectType) == false) return false;

        effectName = hitEffect[effectType];
        return true;
    }
}
