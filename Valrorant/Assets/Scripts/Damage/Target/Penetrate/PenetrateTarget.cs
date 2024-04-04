using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class PenetrateTarget : MonoBehaviour, IPenetrable, IEffectable
{
    protected float _durability;

    protected Dictionary<IEffectable.ConditionType, IEffectable.EffectName> _hitEffect;

    protected abstract void Initialize();

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    public float ReturnDurability()
    {
        return _durability;
    }

    public bool CanReturnHitEffectName(IEffectable.ConditionType effectType)
    {
        if (_hitEffect.ContainsKey(effectType) == false) return false;

        return true;
    }

    public string ReturnHitEffectName(IEffectable.ConditionType effectType)
    {
        return _hitEffect[effectType].ToString();
    }

    public GameObject ReturnAttachedObject()
    {
        return gameObject;
    }
}
