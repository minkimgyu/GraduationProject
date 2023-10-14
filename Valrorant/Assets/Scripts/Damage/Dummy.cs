using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour, IDamageable
{
    public void GetDamage(float damage, Vector3 hitPosition, Vector3 hitNormal)
    {
        BaseEffect effect;
        effect = ObjectPooler.SpawnFromPool<BaseEffect>("DamageTxt");

        effect.Initialize(hitPosition, hitNormal, Quaternion.LookRotation(-hitNormal), damage);
        effect.PlayEffect();
    }
}
