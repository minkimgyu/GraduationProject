using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

public interface IHitable
{
    IDamageable IDamage { get; set; }

    void OnHit(float damage, Vector3 hitPosition, Vector3 hitNormal);

    void SpawnDamageTxt(float damage, Vector3 hitPosition, Vector3 hitNormal);

    // 여기서 데미지 텍스트 생성 코드 넣자

    DistanceAreaData.HitArea ReturnHitArea();
}
