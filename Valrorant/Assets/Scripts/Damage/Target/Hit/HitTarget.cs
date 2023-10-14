using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

public class HitTarget : MonoBehaviour, IPenetrable, IHitable
{
    [SerializeField]
    float durability = 0;

    [SerializeField]
    string bulletPenetrationEffectName;

    [SerializeField]
    string BulletNonPenetrationEffectName;

    [SerializeField]
    string knifeAttackEffectName;

    protected Dictionary<IPenetrable.HitEffectType, string> hitEffect;

    public IDamageable IDamage { get; set; }

    [SerializeField]
    DistanceAreaData.HitArea hitArea;

    protected virtual void Start()
    {
        IDamage = GetComponentInParent<IDamageable>();
        hitEffect = new Dictionary<IPenetrable.HitEffectType, string>()
        {
            {IPenetrable.HitEffectType.BulletPenetration, bulletPenetrationEffectName},
            {IPenetrable.HitEffectType.BulletNonPenetration, BulletNonPenetrationEffectName},
            {IPenetrable.HitEffectType.KnifeAttack, knifeAttackEffectName}
        };
    }

    public GameObject ReturnPenetrableTarget()
    {
        return gameObject;
    }

    public float ReturnDurability()
    {
        return durability;
    }

    public void OnHit(float damage, Vector3 hitPosition, Vector3 hitNormal)
    {
        IDamage.GetDamage(damage, hitPosition, hitNormal);
    }

    public bool CanReturnHitEffectName(IPenetrable.HitEffectType effectType, out string effectName)
    {
        effectName = "";
        if (hitEffect.ContainsKey(effectType) == false) return false;

        effectName = hitEffect[effectType];
        return true;
    }

    public DistanceAreaData.HitArea ReturnHitArea()
    {
        return hitArea;
    }
}
