using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    Vector3 GetFowardVector();

    void GetDamage(float damage);
}
