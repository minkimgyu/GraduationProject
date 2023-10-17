using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

public class BaseHitTarget : MonoBehaviour, IPenetrable, IEffectable, IHitable
{
    [SerializeField]
    bool canSpawnDamageTxt = true;

    [SerializeField]
    float durability = 0;

    [SerializeField]
    string bulletPenetrationEffectName;

    [SerializeField]
    string BulletNonPenetrationEffectName;

    [SerializeField]
    string knifeAttackEffectName;

    Dictionary<IEffectable.ConditionType, string> hitEffect;

    public IDamageable IDamage { get; set; }

    [SerializeField]
    DistanceAreaData.HitArea hitArea;

    protected virtual void Start()
    {
        hitEffect = new Dictionary<IEffectable.ConditionType, string>()
        {
            {IEffectable.ConditionType.BulletPenetration, bulletPenetrationEffectName},
            {IEffectable.ConditionType.BulletNonPenetration, BulletNonPenetrationEffectName},
            {IEffectable.ConditionType.KnifeAttack, knifeAttackEffectName}
        };
    }

    public float ReturnDurability()
    {
        return durability;
    }

    public void OnHit(float damage, Vector3 hitPosition, Vector3 hitNormal)
    {
        IDamage.GetDamage(damage);
        SpawnDamageTxt(damage, hitPosition, hitNormal);
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

    public DistanceAreaData.HitArea ReturnHitArea()
    {
        return hitArea;
    }

    public GameObject ReturnAttachedObject()
    {
        return gameObject;
    }

    public void SpawnDamageTxt(float damage, Vector3 hitPosition, Vector3 hitNormal)
    {
        if (canSpawnDamageTxt == false) return;

        BaseEffect effect;
        effect = ObjectPooler.SpawnFromPool<BaseEffect>("DamageTxt");

        effect.Initialize(hitPosition, hitNormal, Quaternion.LookRotation(-hitNormal), damage);
        effect.PlayEffect();
    }
}
