using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

abstract public class ResultStrategy
{
    public abstract void Do();
    protected virtual void SpawnHitEffect(string name, Vector3 hitPosition, Vector3 hitNormal) { }
    protected virtual void SpawnHitEffect(string name, Vector3 hitPosition, Vector3 hitNormal, bool isBlocked) { }
}

public class NoAttack : ResultStrategy
{
    public override void Do() { }
}

public class Zoom : ResultStrategy
{
    // 스크립터블 오브젝트로 구현하기

    AimEvent _aimEvent;
    GameObject _scope;
    float _zoomDuration;
    float _scopeOnDelay;

    bool nowZoom = false;

    public Zoom(AimEvent aimEvent, GameObject scope, float zoomDuration, float scopeOnDuration)
    {
        _aimEvent = aimEvent;
        _scope = scope;
        _zoomDuration = zoomDuration;
        _scopeOnDelay = scopeOnDuration;
    }

    public override void Do()
    {
        nowZoom = !nowZoom;
        _aimEvent.RaiseEvent(_scope, nowZoom, _zoomDuration, _scopeOnDelay);
    }
}


abstract public class ApplyAttack : ResultStrategy
{
    protected Transform _camTransform;

    protected float _range;
    protected string _hitEffect;
    protected int _targetLayer;
    protected BaseDamageConverter _damageConverter;

    public ApplyAttack(Transform camTransform, float range, string hitEffect, int targetLayer)
    {
        _camTransform = camTransform;
        _range = range;
        _hitEffect = hitEffect;
        _targetLayer = targetLayer;
    }

    protected void DrawGoDebugLine(Vector3 offset = default(Vector3))
    {
        Debug.DrawRay(_camTransform.position, (_camTransform.forward + offset) * _range, Color.green, 10); // 디버그 레이를 카메라가 바라보고 있는 방향으로 발사한다.
    }

    protected void DrawBackDebugLine(Vector3 offset = default(Vector3))
    {
        Debug.DrawRay(_camTransform.position + (_camTransform.forward + offset) * _range, -(_camTransform.forward + offset) * _range, Color.red, 10); // 디버그 레이를 카메라가 바라보고 있는 방향으로 발사한다.
    }
}

abstract public class BaseGunAttack : ApplyAttack
{
    protected Transform _muzzle;

    protected float _penetratePower;

    protected float _minDecreaseRatio = 0.05f;
    protected float _trajectoryLineOffset = 1.3f;

    protected string _nonPenetrateHitEffect;
    protected string _trajectoryLineEffect;

    public BaseGunAttack(Transform camTransform, float range, string hitEffect, int targetLayer, Transform muzzle, float penetratePower,
        string nonPenetrateHitEffect, string trajectoryLineEffect, Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> damageDictionary) : base(camTransform, range, hitEffect, targetLayer)
    {
        _muzzle = muzzle;
        _penetratePower = penetratePower;

        _nonPenetrateHitEffect = nonPenetrateHitEffect;
        _trajectoryLineEffect = trajectoryLineEffect;

        _damageConverter = new DistanceAreaBasedDamageConverter(damageDictionary);
    }

    protected List<PenetrateData> ReturnPenetrateData(Vector3 offset = default(Vector3))
    {
        RaycastHit[] entryHits;
        entryHits = Physics.RaycastAll(_camTransform.position, _camTransform.forward + offset, _range, _targetLayer);

        if (entryHits.Length == 0) return null; // 만약 아무도 맞지 않았다면 리턴

        System.Array.Sort(entryHits, (x, y) => x.distance.CompareTo(y.distance)); // 맞은 객체들을 거리 기반으로 정렬해준다.

        Vector3 endPoint = _camTransform.position + (_camTransform.forward + offset) * _range; // 레이캐스트가 닫는 가장 마지막 위치

        RaycastHit[] exitHits;
        exitHits = Physics.RaycastAll(endPoint, -(_camTransform.forward + offset), _range, _targetLayer); // 다시 반대로 쏴줌

        DrawBackDebugLine(offset);

        System.Array.Sort(exitHits, (x, y) => y.distance.CompareTo(x.distance)); // 맞은 객체들을 거리 기반으로 정렬해준다.

        List<PenetrateData> penetrateDatas = new List<PenetrateData>();

        for (int i = 0; i < entryHits.Length; i++) // 정렬 순서로 
        {
            for (int j = 0; j < exitHits.Length; j++)
            {
                if(entryHits[i].collider == exitHits[j].collider)
                {
                    IPenetrable target;
                    entryHits[i].collider.TryGetComponent(out target);
                    if (target == null) continue;

                    float distanceFromStartPoint = Vector3.Distance(_camTransform.position, entryHits[i].point);

                    penetrateDatas.Add(new PenetrateData(distanceFromStartPoint, entryHits[i].point, exitHits[j].point, entryHits[i].normal, exitHits[j].normal, target));
                    break;
                }
            }
        }

        return penetrateDatas;
    }

    protected void CalculateDamage(IHitable hit, PenetrateData data, float decreaseRatio)
    {
        float damage = _damageConverter.ReturnDamage(hit.ReturnHitArea(), data.DistanceFromStartPoint);

        if (decreaseRatio > _minDecreaseRatio) // decreaseRatio가 5% 이상인 경우만 해당
        {
            damage -= Mathf.Round(damage * decreaseRatio);
        }

        hit.OnHit(damage, data.EntryPoint, data.EntryNormal); // 데미지 적용
    }

    protected void CalculatePenetratePower(List<PenetrateData> penetrateDatas)
    {
        float tmpPenetratePower = _penetratePower;

        string effectName;

        List<IDamageable> alreadyDamagedObjects = new List<IDamageable>();

        for (int i = 0; i < penetrateDatas.Count; i++)
        {
            float finalDurability = penetrateDatas[i].ReturnFinalDurability();


            GameObject target = penetrateDatas[i].Target.ReturnPenetrableTarget();
            IHitable hit;

            target.TryGetComponent(out hit);
            if (hit != null)
            {
                if (alreadyDamagedObjects.Contains(hit.IDamage))
                {
                    continue;
                }
                else
                {
                    alreadyDamagedObjects.Add(hit.IDamage);
                }
            }

            if (tmpPenetratePower - finalDurability >= 0)
            {
                bool canReturnEffectName = penetrateDatas[i].Target.CanReturnHitEffectName(IPenetrable.HitEffectType.BulletPenetration, out effectName);
                if (canReturnEffectName)
                {
                    SpawnHitEffect(effectName, penetrateDatas[i].EntryPoint, penetrateDatas[i].EntryNormal, false);
                    SpawnHitEffect(effectName, penetrateDatas[i].ExitPoint, penetrateDatas[i].ExitNormal, false);
                    // 이 경우, 진입, 퇴장 포인트 모두에 관통 이미지 추가
                }

                tmpPenetratePower -= finalDurability;

                if (i == penetrateDatas.Count - 1) // 만약 관통이 마지막 충돌일 경우
                {
                    Vector3 offsetDir = (penetrateDatas[i].ExitPoint - _muzzle.position).normalized * _trajectoryLineOffset;
                    DrawTrajectoryLine(penetrateDatas[i].ExitPoint + offsetDir);
                }
            }
            else
            {
                bool canReturnEffectName = penetrateDatas[i].Target.CanReturnHitEffectName(IPenetrable.HitEffectType.BulletNonPenetration, out effectName);
                if(canReturnEffectName) SpawnHitEffect(effectName, penetrateDatas[i].EntryPoint, penetrateDatas[i].EntryNormal, true);

                DrawTrajectoryLine(penetrateDatas[i].EntryPoint);
                break; // 여기서 끝내줌
                // 이 경우, 막혔기 때문에 이 포인트까지 총알 궤적 그리기
                // 이 포인트에 총알이 막힌 이미지 추가
            }
            // 위 과정에서 데미지 감소 효과 추가


            float decreasePowerRatio = (_penetratePower - tmpPenetratePower) / _penetratePower;

            if (hit != null)
            {
                CalculateDamage(hit, penetrateDatas[i], decreasePowerRatio);
            }

            if (penetrateDatas.Count <= i + 1) continue; // 뒤에 관통 정보가 없다면 진행하지 않음

            float distanceBetweenExitAndEntryPoint = Vector3.Distance(penetrateDatas[i].ExitNormal, penetrateDatas[i + 1].EntryNormal);
            float finalDistanceBetweenExitAndEntryPoint = distanceBetweenExitAndEntryPoint * penetrateDatas[i].AirDurability;

            tmpPenetratePower -= finalDistanceBetweenExitAndEntryPoint;
        }
    }

    protected override void SpawnHitEffect(string name, Vector3 hitPosition, Vector3 hitNormal, bool isBlocked)
    {
        BaseEffect bulletHoleEffect;
        bulletHoleEffect = ObjectPooler.SpawnFromPool<BaseEffect>(name);

        bulletHoleEffect.Initialize(hitPosition, hitNormal, Quaternion.LookRotation(-hitNormal));
        bulletHoleEffect.PlayEffect();
    }

    void DrawTrajectoryLine(Vector3 hitPosition)
    {
        TrajectoryLineEffect trajectoryLineEffect = ObjectPooler.SpawnFromPool<TrajectoryLineEffect>(_trajectoryLineEffect);
        trajectoryLineEffect.Initialize(hitPosition, _muzzle.position);
        trajectoryLineEffect.PlayEffect();
    }
}

public class SingleProjectileAttack : BaseGunAttack
{
    // NormalGun, ScatterGun 버리고 여기에 공격 함수 구현

    public SingleProjectileAttack(Transform camTransform, float range, string hitEffect, int targetLayer, Transform muzzle, float penetratePower,
        string nonPenetrateHitEffect, string trajectoryLineEffect, Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> damageDictionary)
        : base(camTransform, range, hitEffect, targetLayer, muzzle, penetratePower, nonPenetrateHitEffect, trajectoryLineEffect, damageDictionary)
    {
    }

    public override void Do()
    {
        //if (CanShoot() == false) return;

        DrawGoDebugLine();

        List<PenetrateData> penetrateDatas = ReturnPenetrateData();
        if (penetrateDatas == null) return;

        CalculatePenetratePower(penetrateDatas);
    }
}

public class ScatterProjectileGunAttack : BaseGunAttack
{
    int _projectileCounts;

    float _spreadOffset;

    public ScatterProjectileGunAttack(Transform camTransform, float range, string hitEffect, int targetLayer, Transform muzzle, float penetratePower,
       string nonPenetrateHitEffect, string trajectoryLineEffect, int projectileCounts, float spreadOffset, Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> damageDictionary)
       : base(camTransform, range, hitEffect, targetLayer, muzzle, penetratePower, nonPenetrateHitEffect, trajectoryLineEffect, damageDictionary)
    {
        _projectileCounts = projectileCounts;
        _spreadOffset = spreadOffset;
    }

    List<Vector3> ReturnOffsetDistance()
    {
        List<Vector3> offsetDistance = new List<Vector3>();

        for (int i = 0; i < _projectileCounts; i++)
        {
            float x = Random.Range(-_spreadOffset, _spreadOffset);
            float y = Random.Range(-_spreadOffset, _spreadOffset);
            offsetDistance.Add(new Vector3(0, y, x));
        }

        return offsetDistance;
    }

    public override void Do()
    {
        List<Vector3> offsetDistances = ReturnOffsetDistance();

        for (int i = 0; i < offsetDistances.Count; i++)
        {
            DrawGoDebugLine(offsetDistances[i]);

            List<PenetrateData> penetrateDatas = ReturnPenetrateData(offsetDistances[i]);
            if (penetrateDatas == null) continue;

            CalculatePenetratePower(penetrateDatas);
        }
    }
}

public class KnifeAttack : ApplyAttack
{
    // 칼 공격은 여기서 구현
    public KnifeAttack(Transform camTransform, float range, string hitEffect, int targetLayer, DirectionData directionData) : base(camTransform, range, hitEffect, targetLayer)
    {
        _damageConverter = new DirectionBasedDamageConverter(directionData);
    }

    void CheckRaycastHit()
    {
        RaycastHit hit;
        Physics.Raycast(_camTransform.position, _camTransform.forward, out hit, _range, _targetLayer);

        if (hit.collider == null) return;

        IPenetrable penetrable;
        hit.collider.TryGetComponent(out penetrable);

        if (penetrable == null) return;
        string effectName;

        bool canReturnEffectName = penetrable.CanReturnHitEffectName(IPenetrable.HitEffectType.KnifeAttack, out effectName);
        if(canReturnEffectName) SpawnHitEffect(effectName, hit.point, hit.normal);

        IDamageable damageable;
        hit.collider.TryGetComponent(out damageable);


        float damage = _damageConverter.ReturnDamage(_camTransform.position, hit.collider.transform.position);

        if (damageable == null) return;
        damageable.GetDamage(damage, hit.point, hit.normal);
    }

    public override void Do()
    {
        DrawGoDebugLine();
        CheckRaycastHit();
    }

    protected override void SpawnHitEffect(string name, Vector3 hitPosition, Vector3 hitNormal)
    {
        BaseEffect bulletHoleEffect;
        bulletHoleEffect = ObjectPooler.SpawnFromPool<BaseEffect>(name);

        bulletHoleEffect.Initialize(hitPosition, hitNormal, Quaternion.LookRotation(-hitNormal));
        bulletHoleEffect.PlayEffect();
    }
}
