using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

public class BaseHitTarget : MonoBehaviour, IPenetrable, IEffectable, IHitable
{
    [SerializeField] bool canSpawnDamageTxt = true;

    protected float _durability = 0;

    Dictionary<IEffectable.ConditionType, IEffectable.EffectName> hitEffect;

    public IDamageable IDamage { get; set; }

    protected HitArea hitArea;

    protected virtual void Start()
    {
        hitEffect = new Dictionary<IEffectable.ConditionType, IEffectable.EffectName>()
        {
            {IEffectable.ConditionType.Penetration, IEffectable.EffectName.ObjectFragmentation},
            {IEffectable.ConditionType.NonPenetration, IEffectable.EffectName.ObjectFragmentation},
            {IEffectable.ConditionType.Stabbing, IEffectable.EffectName.ObjectFragmentation}
        };
    }

    public float ReturnDurability()
    {
        return _durability;
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
        return hitEffect[effectType].ToString();
    }

    public HitArea ReturnHitArea()
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
