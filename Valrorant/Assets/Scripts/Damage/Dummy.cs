using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour, IDamageable
{
    public float HP { get; set; }

    public void GetDamage(float damage)
    {
    }

    public Vector3 GetFowardVector()
    {
        return transform.forward;
    }
}
