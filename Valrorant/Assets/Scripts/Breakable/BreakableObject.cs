using DamageUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : DirectDamageTarget, IDamageable
{
    [SerializeField] float _maxHp;

    bool breakOnce = true;

    [SerializeField]
    float _disableDuration;

    [SerializeField]
    GameObject fracturedObjectPrefab;

    public float HP { get; set; }

    protected override void Start()
    {
        base.Start();
        HP = _maxHp;
    }

    public void GetDamage(float damage)
    {
        HP -= damage;
        if(IsDie() && breakOnce == true)
        {
            Die();
        }
    }

    public Vector3 GetFowardVector()
    {
        return transform.forward;
    }

    public bool IsDie()
    {
        return HP <= 0;
    }

    public void Die()
    {
        FracturedObject fracturedObject;

        breakOnce = false;
        GameObject go = Instantiate(fracturedObjectPrefab, transform.position, transform.rotation);
        go.TryGetComponent(out fracturedObject);
        if (fracturedObject == null) return;

        fracturedObject.Initialize(_disableDuration);
        Destroy(gameObject);
    }
}
