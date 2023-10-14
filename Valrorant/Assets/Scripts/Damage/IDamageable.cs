using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void GetDamage(float damage, Vector3 hitPosition, Vector3 hitNormal);
}
