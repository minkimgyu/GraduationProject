using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;
using ObserverPattern;

abstract public class ResultStrategy
{
    public virtual void Do() { }
    public virtual void Do(float bulletSpreadPower) { }
    public virtual void Do(bool nowZoom, bool useInstantly) { }

    protected virtual void SpawnHitEffect(string name, Vector3 hitPosition, Vector3 hitNormal) { }
    protected virtual void SpawnHitEffect(string name, Vector3 hitPosition, Vector3 hitNormal, bool isBlocked) { }

    public abstract void OnUpdate();
}

public class NoAttack : ResultStrategy
{
    public override void Do() { }

    public override void OnUpdate() { }
}

public class Zoom : ResultStrategy, ISubject<GameObject, bool, float, float, float, float, bool>
{
    GameObject _scope;
    float _zoomDuration;
    float _scopeOnDelay;

    float _normalFieldOfView;
    float _zoomFieldOfView;

    bool nowZoom = false;

    public Zoom(GameObject scope, float zoomDuration, float scopeOnDuration, float normalFieldOfView, float zoomFieldOfView)
    {
        _scope = scope;
        _zoomDuration = zoomDuration;
        _scopeOnDelay = scopeOnDuration;

        _normalFieldOfView = normalFieldOfView;
        _zoomFieldOfView = zoomFieldOfView;

        Observers = new List<IObserver<GameObject, bool, float, float, float, float, bool>>();
    }

    public List<IObserver<GameObject, bool, float, float, float, float, bool>> Observers { get; set; }

    public void AddObserver(IObserver<GameObject, bool, float, float, float, float, bool> observer)
    {
        Observers.Add(observer);
    }

    public void RemoveObserver(IObserver<GameObject, bool, float, float, float, float, bool> observer)
    {
        Observers.Remove(observer);
    }

    public void NotifyToObservers(GameObject scope, bool nowZoom, float zoomDuration, float scopeOnDelay, float normalFieldOfView, float zoomFieldOfView, bool isInstant = false)
    {
        for (int i = 0; i < Observers.Count; i++)
        {
            Observers[i].Notify(scope, nowZoom, zoomDuration, scopeOnDelay, normalFieldOfView, zoomFieldOfView, isInstant);
        }
    }

    public override void Do() 
    {
        nowZoom = !nowZoom;
        NotifyToObservers(_scope, nowZoom, _zoomDuration, _scopeOnDelay, _normalFieldOfView, _zoomFieldOfView);
    }

    public override void Do(bool nowZoom, bool useInstantly)
    {
        NotifyToObservers(_scope, nowZoom, _zoomDuration, _scopeOnDelay, _normalFieldOfView, _zoomFieldOfView, useInstantly);
    }

    public override void OnUpdate() { }
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

    // 데미지를 적용하는 함수는 여기에 만들어주기 ex) 총기형 데미지 함수, 칼 데미지 함수
    protected virtual float CalculateDamage(IHitable hitable, PenetrateData data, float decreaseRatio) { return default; }

    protected virtual float CalculateDamage(IHitable hitable) { return default; }



    protected virtual void ApplyDamage(IHitable hitable, RaycastHit hit) { }

    protected virtual void ApplyDamage(IHitable hitable, PenetrateData data, float decreaseRatio) { }


    protected void DrawPenetrateLine(Vector3 hitPoint)
    {
        float diatance = Vector3.Distance(_camTransform.position, hitPoint);
        Vector3 direction = (hitPoint - _camTransform.position).normalized;

        Debug.DrawRay(_camTransform.position, direction * diatance, Color.green, 10); // 디버그 레이를 카메라가 바라보고 있는 방향으로 발사한다.
    }
}

abstract public class PenetrateAttack : ApplyAttack
{
    protected Transform _muzzle;

    protected float _penetratePower;

    protected float _minDecreaseRatio = 0.05f;
    protected float _trajectoryLineOffset = 1.3f;

    protected string _nonPenetrateHitEffect;
    protected string _trajectoryLineEffect;

    public PenetrateAttack(Transform camTransform, float range, string hitEffect, int targetLayer, Transform muzzle, float penetratePower,
        string nonPenetrateHitEffect, string trajectoryLineEffect, Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> damageDictionary) : base(camTransform, range, hitEffect, targetLayer)
    {
        _muzzle = muzzle;
        _penetratePower = penetratePower;

        _nonPenetrateHitEffect = nonPenetrateHitEffect;
        _trajectoryLineEffect = trajectoryLineEffect;

        _damageConverter = new DistanceAreaBasedDamageConverter(damageDictionary);
    }

    RaycastHit[] DetectCollider(Vector3 origin, Vector3 direction, float maxDistance, int layer)
    {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(origin, direction, maxDistance, layer);

        System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance)); // 맞은 객체들을 거리 기반으로 정렬해준다.

        return hits;
    }

    protected List<PenetrateData> ReturnPenetrateData(Vector3 offset = default(Vector3))
    {
        RaycastHit[] entryHits = DetectCollider(_camTransform.position, _camTransform.forward + offset, _range, _targetLayer);

        if (entryHits.Length == 0) return null; // 만약 아무도 맞지 않았다면 리턴

        Vector3 endPoint = _camTransform.position + (_camTransform.forward + offset) * _range; // 레이캐스트가 닫는 가장 마지막 위치

        RaycastHit[] exitHits = DetectCollider(endPoint, -(_camTransform.forward + offset), _range, _targetLayer);

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

    protected override float CalculateDamage(IHitable hit, PenetrateData data, float decreaseRatio)
    {
        float damage = _damageConverter.ReturnDamage(hit.ReturnHitArea(), data.DistanceFromStartPoint);

        if (decreaseRatio > _minDecreaseRatio) // decreaseRatio가 5% 이상인 경우만 해당
        {
            damage -= Mathf.Round(damage * decreaseRatio);
        }

        return damage;
    }

    protected override void ApplyDamage(IHitable hit, PenetrateData data, float decreaseRatio)
    {
        float damage = CalculateDamage(hit, data, decreaseRatio);
        hit.OnHit(damage, data.EntryPoint, data.EntryNormal); // 데미지 적용
    }

    bool CheckIsAlreadyDamaged(IHitable hit, List<IDamageable> alreadyDamagedObjects)
    {
        if (alreadyDamagedObjects.Contains(hit.IDamage))
        {
            return true;
        }
        else
        {
            alreadyDamagedObjects.Add(hit.IDamage);
            return false;
        }
    }

    void DrawPenetrateLineUntilExitPoint(PenetrateData penetrateData)
    {
        Vector3 offsetDir = (penetrateData.ExitPoint - _muzzle.position).normalized * _trajectoryLineOffset;
        DrawTrajectoryLine(penetrateData.ExitPoint + offsetDir);
        DrawPenetrateLine(penetrateData.ExitPoint);
    }

    void DrawPenetrateLineUntilEntryPoint(PenetrateData penetrateData)
    {
        DrawTrajectoryLine(penetrateData.EntryPoint);
        DrawPenetrateLine(penetrateData.EntryPoint);
    }

    void SpawnBothPenetrateEffect(IEffectable effectableTarget, PenetrateData penetrateData)
    {
        bool canReturnEffectName = effectableTarget.CanReturnHitEffectName(IEffectable.ConditionType.BulletPenetration);
        if (canReturnEffectName)
        {
            string effectName = effectableTarget.ReturnHitEffectName(IEffectable.ConditionType.BulletPenetration);

            SpawnHitEffect(effectName, penetrateData.EntryPoint, penetrateData.EntryNormal, false);
            SpawnHitEffect(effectName, penetrateData.ExitPoint, penetrateData.ExitNormal, false);
            // 이 경우, 진입, 퇴장 포인트 모두에 관통 이미지 추가
        }
    }

    void SpawnNonePenetrateEffect(IEffectable effectableTarget, PenetrateData penetrateData)
    {
        bool canReturnEffectName = effectableTarget.CanReturnHitEffectName(IEffectable.ConditionType.BulletNonPenetration);
        string effectName = effectableTarget.ReturnHitEffectName(IEffectable.ConditionType.BulletNonPenetration);

        if (canReturnEffectName) SpawnHitEffect(effectName, penetrateData.EntryPoint, penetrateData.EntryNormal, true);
    }


    float CalculatePenetratePower(float penetratePower, float durability, IEffectable effectableTarget, PenetrateData penetrateData, bool isLastContact)
    {
        if (penetratePower - durability >= 0)
        {
            SpawnBothPenetrateEffect(effectableTarget, penetrateData);

            penetratePower -= durability;
            if (isLastContact == true) DrawPenetrateLineUntilExitPoint(penetrateData);

            return penetratePower;
        }

        SpawnNonePenetrateEffect(effectableTarget, penetrateData);
        DrawPenetrateLineUntilEntryPoint(penetrateData);
        return 0;
    }

    float CalculateAirThroughPenetratePower(float penetratePower, Vector3 exitPoint, Vector3 entryPoint, float airDurability)
    {
        float distanceBetweenExitAndEntryPoint = Vector3.Distance(exitPoint, entryPoint);
        float finalDistanceBetweenExitAndEntryPoint = distanceBetweenExitAndEntryPoint * airDurability;

        penetratePower -= finalDistanceBetweenExitAndEntryPoint;

        return penetratePower;
    }

    protected void ProgressPenetrateSequence(List<PenetrateData> penetrateDatas)
    {
        float tmpPenetratePower = _penetratePower;

        List<IDamageable> alreadyDamagedObjects = new List<IDamageable>();

        for (int i = 0; i < penetrateDatas.Count; i++)
        {
            // 관통된 오브젝트를 가져옴
            GameObject target = penetrateDatas[i].Target.ReturnAttachedObject();

            // IHitable 가져옴
            IHitable hitable;
            target.TryGetComponent(out hitable);

            //IEffectable 가져옴
            IEffectable effectable;
            target.TryGetComponent(out effectable);
            if (effectable == null) continue;


            if(hitable != null)
            {
                // 이미 해당 오브젝트에 데미지가 적용되었는지 확인
                bool isAlreadyDamaged = CheckIsAlreadyDamaged(hitable, alreadyDamagedObjects);
                if (isAlreadyDamaged) continue;
            }
           

            // 오브젝트의 내구도를 가져옴
            float finalDurability = penetrateDatas[i].ReturnFinalDurability();

            bool IsLastContact = false;
            if (i == penetrateDatas.Count - 1) IsLastContact = true; // 마지막 충돌 시
           
            // 관통 수치가 0이면 break
            tmpPenetratePower = CalculatePenetratePower(tmpPenetratePower, finalDurability, effectable, penetrateDatas[i], IsLastContact);
            if (tmpPenetratePower == 0) break;



            if (hitable != null)
            {
                // 데미지 감소 적용
                float decreasePowerRatio = (_penetratePower - tmpPenetratePower) / _penetratePower;
                if (hitable != null) ApplyDamage(hitable, penetrateDatas[i], decreasePowerRatio);
            }



            if (penetrateDatas.Count <= i + 1) continue; // 뒤에 관통 정보가 없다면 진행하지 않음

            // 다음으로 관통된 오브젝트 사이의 거리에 따라 관통 수치에 감소 적용
            tmpPenetratePower = CalculateAirThroughPenetratePower(tmpPenetratePower, penetrateDatas[i].ExitPoint, penetrateDatas[i + 1].EntryPoint, PenetrateData.AirDurability);
        }
    }

    protected void Shoot(Vector3 offset = default(Vector3))
    {
        List<PenetrateData> penetrateDatas = ReturnPenetrateData(offset);
        if (penetrateDatas == null) return;

        ProgressPenetrateSequence(penetrateDatas);
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

public class SingleProjectileAttack : PenetrateAttack
{
    // NormalGun, ScatterGun 버리고 여기에 공격 함수 구현

    public SingleProjectileAttack(Transform camTransform, float range, string hitEffect, int targetLayer, Transform muzzle, float penetratePower,
        string nonPenetrateHitEffect, string trajectoryLineEffect, Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> damageDictionary)
        : base(camTransform, range, hitEffect, targetLayer, muzzle, penetratePower, nonPenetrateHitEffect, trajectoryLineEffect, damageDictionary)
    {
    }

    protected Vector3 ReturnOffset(float weight)
    {
        float x = Random.Range(-weight, weight);
        float y = Random.Range(-weight, weight);

        return new Vector3(0, y, x);
    }

    public override void Do(float bulletSpreadPower)
    {
        Vector3 offset = ReturnOffset(bulletSpreadPower);
        Shoot(offset);
    }

    public override void OnUpdate()
    {
    }
}

public class SingleProjectileAttackWithWeight : SingleProjectileAttack // 가중치가 적용되는 공격
{
    WeightApplier _weightApplier;

    public SingleProjectileAttackWithWeight(Transform camTransform, float range, string hitEffect, int targetLayer, Transform muzzle, float penetratePower,
        string nonPenetrateHitEffect, string trajectoryLineEffect, Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> damageDictionary, WeightApplier weightApplier)
        : base(camTransform, range, hitEffect, targetLayer, muzzle, penetratePower, nonPenetrateHitEffect, trajectoryLineEffect, damageDictionary)
    {
        _weightApplier = weightApplier;
    }

    public override void Do(float bulletSpreadPower)
    {
        base.Do(_weightApplier.StoredWeight + bulletSpreadPower);
        _weightApplier.MultiplyWeight();
    }

    public override void OnUpdate()
    {
        _weightApplier.OnUpdate();
    }
}

public class ScatterProjectileGunAttackWithWeight : PenetrateAttack // 산탄은 가중치가 적용되지 않음
{
    int _projectileCounts;

    float _spreadOffset;
    float bulletSpreadPowerDecreaseRatio = 0.35f;

    WeightApplier _weightApplier;

    public ScatterProjectileGunAttackWithWeight(Transform camTransform, float range, string hitEffect, int targetLayer, Transform muzzle, float penetratePower,
       string nonPenetrateHitEffect, string trajectoryLineEffect, int projectileCounts, float spreadOffset, Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> damageDictionary, WeightApplier weightApplier)
       : base(camTransform, range, hitEffect, targetLayer, muzzle, penetratePower, nonPenetrateHitEffect, trajectoryLineEffect, damageDictionary)
    {
        _projectileCounts = projectileCounts;
        _spreadOffset = spreadOffset;
        _weightApplier = weightApplier;
    }

    List<Vector3> ReturnOffsetDistance(float weight)
    {
        List<Vector3> offsetDistance = new List<Vector3>();

        for (int i = 0; i < _projectileCounts; i++)
        {
            float x = Random.Range(-_spreadOffset - weight, _spreadOffset + weight);
            float y = Random.Range(-_spreadOffset - weight, _spreadOffset + weight);
            offsetDistance.Add(new Vector3(0, y, x));
        }

        return offsetDistance;
    }

    public override void Do(float bulletSpreadPower)
    {
        List<Vector3> offsetDistances = ReturnOffsetDistance(_weightApplier.StoredWeight + bulletSpreadPower * bulletSpreadPowerDecreaseRatio);

        for (int i = 0; i < offsetDistances.Count; i++)
        {
            Shoot(offsetDistances[i]);
            _weightApplier.MultiplyWeight();
        }
    }

    public override void OnUpdate()
    {
        _weightApplier.OnUpdate();
    }
}

public class KnifeAttack : ApplyAttack
{
    // 칼 공격은 여기서 구현
    public KnifeAttack(Transform camTransform, float range, string hitEffect, int targetLayer, DirectionData directionData) : base(camTransform, range, hitEffect, targetLayer)
    {
        _damageConverter = new DirectionBasedDamageConverter(directionData);
    }

    protected override float CalculateDamage(IHitable hitable)
    {
        return _damageConverter.ReturnDamage(_camTransform.forward, hitable.IDamage.GetFowardVector());
    }

    protected override void ApplyDamage(IHitable hitable, RaycastHit hit)
    {
        float damage = CalculateDamage(hitable);
        hitable.OnHit(damage, hit.point, hit.normal);
    }

    void CheckRaycastHit()
    {
        RaycastHit hit;
        Physics.Raycast(_camTransform.position, _camTransform.forward, out hit, _range, _targetLayer);
        if (hit.collider == null) return;

        SpawnKnifeScratchEffect(hit);

        IHitable hitable;
        hit.collider.TryGetComponent(out hitable);
        if (hitable == null) return;

        ApplyDamage(hitable, hit);
    }

    void SpawnKnifeScratchEffect(RaycastHit hit)
    {
        IEffectable effectable;
        hit.collider.TryGetComponent(out effectable);
        if (effectable == null) return;

        bool canReturnEffectName = effectable.CanReturnHitEffectName(IEffectable.ConditionType.KnifeAttack);
        if (canReturnEffectName)
        {
            string effectName = effectable.ReturnHitEffectName(IEffectable.ConditionType.KnifeAttack);
            SpawnHitEffect(effectName, hit.point, hit.normal);
        }
    }

    public override void Do()
    {
        CheckRaycastHit();
    }

    protected override void SpawnHitEffect(string name, Vector3 hitPosition, Vector3 hitNormal)
    {
        BaseEffect bulletHoleEffect;
        bulletHoleEffect = ObjectPooler.SpawnFromPool<BaseEffect>(name);

        bulletHoleEffect.Initialize(hitPosition, hitNormal, Quaternion.LookRotation(-hitNormal));
        bulletHoleEffect.PlayEffect();
    }

    public override void OnUpdate()
    {
    }
}
