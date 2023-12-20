using DamageUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : DirectDamageTarget, IDamageable
{
    [SerializeField]
    Component component;

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
        FracturedObject fracturedObject;

        HP -= damage;
        if(HP < 0 && breakOnce == true)
        {
            breakOnce = false;
            GameObject go = Instantiate(fracturedObjectPrefab, transform.position, transform.rotation);
            go.TryGetComponent(out fracturedObject);
            if (fracturedObject == null) return;

            fracturedObject.Initialize(_disableDuration);
            Destroy(gameObject);
        }
    }

    public Vector3 GetFowardVector()
    {
        return transform.forward;
    }
}
