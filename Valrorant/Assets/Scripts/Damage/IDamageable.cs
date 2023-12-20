using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    float HP { get; set; }

    Vector3 GetFowardVector();

    void GetDamage(float damage);
}
