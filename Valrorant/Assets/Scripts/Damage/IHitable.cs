using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

public interface IHitable
{
    IDamageable IDamage { get; set; }

    void OnHit(float damage, Vector3 hitPosition, Vector3 hitNormal);

    void SpawnDamageTxt(float damage, Vector3 hitPosition, Vector3 hitNormal);

    // ���⼭ ������ �ؽ�Ʈ ���� �ڵ� ����

    DistanceAreaData.HitArea ReturnHitArea();
}