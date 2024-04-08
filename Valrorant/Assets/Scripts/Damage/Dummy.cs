using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour, IDamageable
{
    public float HP { get; set; }

    public void Die()
    {
    }

    public void GetDamage(float damage)
    {
    }

    public Vector3 GetFowardVector()
    {
        return transform.forward;
    }

    public bool IsDie()
    {
        return HP <= 0;
    }
}
