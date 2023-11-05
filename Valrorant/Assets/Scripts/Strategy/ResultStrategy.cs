using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;
using ObserverPattern;

abstract public class ResultStrategy
{
    public abstract void CheckBulletLeftCount(int leftBulletCount);

    public abstract bool CanDo();

    public abstract void Do();

    // 이벤트를 받는 함수는 abstract 이용해서 함수를 받으면 어떤 기능을 하는지 확실히 보여주기
    public abstract void OnLink(GameObject player);

    public abstract void OnUnLink(GameObject player);

    public abstract int DecreaseBullet(int ammoCountsInMagazine);

    public abstract void OnReload();

    public abstract void OnUnEquip();

    public abstract void OnUpdate();

    public abstract void OnOtherActionEventRequested();
    public abstract int ReturnFireCountInOneShoot();
}

public class NoResult : ResultStrategy
{
    public override void Do() { }

    public override void OnUnLink(GameObject player) { }

    public override int DecreaseBullet(int ammoCountsInMagazine) { return ammoCountsInMagazine; }

    public override void OnReload() { }

    public override void OnUnEquip() { }

    public override void OnUpdate() { }

    public override void OnOtherActionEventRequested() { }

    public override int ReturnFireCountInOneShoot() { return 0; }

    public override void OnLink(GameObject player) { }

    public override bool CanDo() { return false; }

    public override void CheckBulletLeftCount(int leftBullet) { }
}

public class ZoomStrategy : ResultStrategy//, ISubject<GameObject, bool, float, float, float, float, bool>
{
    GameObject _scope;
    float _zoomDuration;
    float _scopeOnDelay;

    float _normalFieldOfView;
    float _zoomFieldOfView;

    bool nowZoom = false;

    Vector3 _zoomCameraPosition;

    public System.Action<GameObject, bool, float, Vector3, float, float, float, bool> OnZoomRequested;
    public System.Action<bool> OnZoomEventCall;

    public ZoomStrategy(GameObject scope, Vector3 zoomCameraPosition, float zoomDuration, float scopeOnDuration, float normalFieldOfView, float zoomFieldOfView,
        System.Action<bool> onZoom)
    {
        _scope = scope;
        _scope.SetActive(false);

        _zoomDuration = zoomDuration;
        _scopeOnDelay = scopeOnDuration;

        _zoomCameraPosition = zoomCameraPosition;

        _normalFieldOfView = normalFieldOfView;
        _zoomFieldOfView = zoomFieldOfView;

        OnZoomEventCall += onZoom;
    }

    public override void Do() 
    {
        nowZoom = !nowZoom;
        OnZoomRequested?.Invoke(_scope, nowZoom, _zoomDuration, _zoomCameraPosition, _scopeOnDelay, _normalFieldOfView, _zoomFieldOfView, false);
        OnZoomEventCall?.Invoke(nowZoom);
    }

    void TurnOffZoomDirectly()
    {
        if (nowZoom == false) return;
        nowZoom = false;

        OnZoomRequested?.Invoke(_scope, nowZoom, _zoomDuration, _zoomCameraPosition, _scopeOnDelay, _normalFieldOfView, _zoomFieldOfView, true);
        OnZoomEventCall?.Invoke(nowZoom);
    }

    public override void OnReload() => TurnOffZoomDirectly();

    public override void OnUnEquip() => TurnOffZoomDirectly();

    public override void OnLink(GameObject player)
    {
        ZoomComponent zoomComponent = player.GetComponent<ZoomComponent>();
        OnZoomRequested += zoomComponent.OnZoomCalled;
    }

    public override void OnUnLink(GameObject player)
    {
        ZoomComponent zoomComponent = player.GetComponent<ZoomComponent>();
        OnZoomRequested -= zoomComponent.OnZoomCalled;
    }

    public override void OnUpdate() { }

    public override int DecreaseBullet(int ammoCountsInMagazine) { return ammoCountsInMagazine; }

    public override void OnOtherActionEventRequested() { }

    public override int ReturnFireCountInOneShoot() { return 0; }

    public override bool CanDo() { return true; }

    public override void CheckBulletLeftCount(int leftBulletCount) { }
}


abstract public class ApplyAttack : ResultStrategy
{
    protected Transform _camTransform;

    protected float _range;
    protected int _targetLayer;
    protected BaseDamageConverter _damageConverter;

    Animator _ownerAnimator;
    Animator _weaponAnimator;
    bool _isMainAction;
    string _weaponName;

    public ApplyAttack(Transform camTransform, float range, int targetLayer, Animator ownerAnimator, Animator weaponAnimator, bool isMainAction, string weaponName)
    {
        _camTransform = camTransform;
        _range = range;
        _targetLayer = targetLayer;

        _ownerAnimator = ownerAnimator;
        _weaponAnimator = weaponAnimator;

        _isMainAction = isMainAction;
        _weaponName = weaponName;
    }

    protected void PlayOwnerAnimation()
    {
        string actionType;
        if (_isMainAction) actionType = "MainAction";
        else actionType = "SubAction";

        _ownerAnimator.Play(_weaponName + actionType, -1, 0);
        _weaponAnimator.Play(actionType, -1, 0);
    }

    protected void PlayAnimation(int index)
    {
        string actionType;
        if (_isMainAction) actionType = "MainAction";
        else actionType = "SubAction";

        _ownerAnimator.Play(_weaponName + actionType + index, -1, 0);
        _weaponAnimator.Play(actionType + index, -1, 0);
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
    protected virtual void SpawnHitEffect(string name, Vector3 hitPosition, Vector3 hitNormal) { }
    protected virtual void SpawnHitEffect(string name, Vector3 hitPosition, Vector3 hitNormal, bool isBlocked) { }


    public override void OnUnEquip() { }

    public override void OnReload() { }
}

abstract public class PenetrateAttack : ApplyAttack, IDisplacement
{
    protected Transform _muzzle;

    protected float _penetratePower;

    protected float _minDecreaseRatio = 0.05f;
    protected float _trajectoryLineOffset = 1.3f;

    protected string _trajectoryLineEffect;

    protected int _fireCountsInOneShoot;

    public float DisplacementWeight { get; set; }

    ParticleSystem _muzzleFlash;
    ParticleSystem _emptyCartridgeSpawner;
    bool _canSpawnMuzzleFlash;

    int _leftBulletCount; // 발사 전 남은 총알 체크

    public PenetrateAttack(Transform camTransform, float range, int targetLayer, int fireCountsInOneShoot, Animator ownerAnimator, Animator weaponAnimator, 
        ParticleSystem muzzleFlashSpawner, bool canSpawnMuzzleFlash, ParticleSystem emptyCartridgeSpawner, bool isMainAction, string weaponName, 
        Transform muzzle, float penetratePower, string trajectoryLineEffect, Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> damageDictionary) 
        : base(camTransform, range, targetLayer, ownerAnimator, weaponAnimator, isMainAction, weaponName)
    {
        _muzzle = muzzle;
        _penetratePower = penetratePower;

        _trajectoryLineEffect = trajectoryLineEffect;
        _fireCountsInOneShoot = fireCountsInOneShoot;

        _muzzleFlash = muzzleFlashSpawner;
        _emptyCartridgeSpawner = emptyCartridgeSpawner;

        _canSpawnMuzzleFlash = canSpawnMuzzleFlash;

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

    void PlayEffect()
    {
        if(_canSpawnMuzzleFlash) _muzzleFlash.Play();
        _emptyCartridgeSpawner.Play();
    }

    public override void OnOtherActionEventRequested() { }

    public override void Do()
    {
        PlayOwnerAnimation();
        PlayEffect();
    }

    public override int ReturnFireCountInOneShoot() { return _fireCountsInOneShoot; }

    public override void OnUnLink(GameObject player)
    {
        MovementComponent movementComponent = player.GetComponent<MovementComponent>();
        movementComponent.OnDisplacementRequested -= OnDisplacementWeightReceived;
    }

    public override void OnLink(GameObject player)
    {
        MovementComponent movementComponent = player.GetComponent<MovementComponent>();
        movementComponent.OnDisplacementRequested += OnDisplacementWeightReceived;
    }

    public void OnDisplacementWeightReceived(float displacement)
    {
        DisplacementWeight = displacement;
    }

    public override int DecreaseBullet(int ammoCountsInMagazine)
    {
        ammoCountsInMagazine -= _fireCountsInOneShoot;
        if (ammoCountsInMagazine < 0) ammoCountsInMagazine = 0;

        return ammoCountsInMagazine;
    }

    public override void CheckBulletLeftCount(int leftBulletCount) { _leftBulletCount = leftBulletCount; }

    public override bool CanDo() { return _leftBulletCount > 0; }
}

// WithWeight는 속도 백터의 길이를 받아서 적용시켜주는 인터페이스
public interface IDisplacement
{
    float DisplacementWeight { get; set; }

    void OnDisplacementWeightReceived(float displacement);
}


// 연사가능한 총에는 탄퍼짐이 없어야함 --> 반동으로 처리하기 때문에
public class SingleProjectileAttack : PenetrateAttack
{
    public SingleProjectileAttack(Transform camTransform, float range, int targetLayer, Animator ownerAnimator,
        Animator weaponAnimator, ParticleSystem muzzleFlashSpawner, bool canSpawnMuzzleFlash, ParticleSystem emptyCartridgeSpawner, 
        bool isMainAction, string weaponName, Transform muzzle, float penetratePower, string trajectoryLineEffect, Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> damageDictionary)
        : base(camTransform, range, targetLayer, 1, ownerAnimator, weaponAnimator, muzzleFlashSpawner, canSpawnMuzzleFlash, emptyCartridgeSpawner, 
            isMainAction, weaponName, muzzle, penetratePower, trajectoryLineEffect, damageDictionary)
    {
    }

    protected Vector3 ReturnOffset(float weight)
    {
        float x = Random.Range(-weight, weight);
        float y = Random.Range(-weight, weight);

        return new Vector3(0, y, x);
    }

    public override void Do()
    {
        base.Do();
        Vector3 multifliedOffset = ReturnOffset(DisplacementWeight);
        Shoot(multifliedOffset);
    }

    public override void OnUpdate()
    {
    }
}

// 점사 가능한 총에는 탄 퍼짐 추가

// WithWeight는 탄 퍼짐을 의미함
public class SingleProjectileAttackWithWeight : SingleProjectileAttack // 가중치가 적용되는 공격
{
    WeightApplier _weightApplier;

    public SingleProjectileAttackWithWeight(Transform camTransform, float range, int targetLayer, Animator ownerAnimator,
        Animator weaponAnimator, ParticleSystem muzzleFlashSpawner, bool canSpawnMuzzleFlash, ParticleSystem emptyCartridgeSpawner,
        bool isMainAction, string weaponName, Transform muzzle, float penetratePower, string trajectoryLineEffect, 
        Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> damageDictionary, WeightApplier weightApplier)
        : base(camTransform, range, targetLayer, ownerAnimator, weaponAnimator, muzzleFlashSpawner, canSpawnMuzzleFlash, emptyCartridgeSpawner, 
            isMainAction, weaponName, muzzle, penetratePower, trajectoryLineEffect, damageDictionary)
    {
        _weightApplier = weightApplier;
    }

    public override void Do()
    {
        DisplacementWeight += _weightApplier.StoredWeight; // 가중치 추가 적용

        Debug.Log(DisplacementWeight);
        Debug.Log(_weightApplier.StoredWeight);
        base.Do();
        _weightApplier.MultiplyWeight();
    }

    public override void OnUpdate()
    {
        _weightApplier.OnUpdate();
    }
}

public class ScatterProjectileGunAttackWithWeight : PenetrateAttack // 산탄은 가중치가 적용되지 않음
{
    float _spreadOffset;
    float bulletSpreadPowerDecreaseRatio = 0.35f;

    int _nextFireCount;

    WeightApplier _weightApplier;

    public ScatterProjectileGunAttackWithWeight(Transform camTransform, float range, int targetLayer, int bulletCountsInOneShoot, Animator ownerAnimator,
        Animator weaponAnimator, ParticleSystem muzzleFlashSpawner, bool canSpawnMuzzleFlash, ParticleSystem emptyCartridgeSpawner,
        bool isMainAction, string weaponName, Transform muzzle, float penetratePower,
       string trajectoryLineEffect, float spreadOffset, Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> damageDictionary, WeightApplier weightApplier)
       : base(camTransform, range, targetLayer, bulletCountsInOneShoot, ownerAnimator, weaponAnimator, muzzleFlashSpawner, canSpawnMuzzleFlash, emptyCartridgeSpawner, 
           isMainAction, weaponName, muzzle, penetratePower, trajectoryLineEffect, damageDictionary)
    {
        _spreadOffset = spreadOffset;
        _weightApplier = weightApplier;
        _nextFireCount = _fireCountsInOneShoot; // 초기 발사 카운트는 _fireCountsInOneShoot를 대입해준다.
    }

    List<Vector3> ReturnOffsetDistance(float weight, int fireCount)
    {
        List<Vector3> offsetDistance = new List<Vector3>();

        for (int i = 0; i < fireCount; i++)
        {
            float x = Random.Range(-_spreadOffset - weight, _spreadOffset + weight);
            float y = Random.Range(-_spreadOffset - weight, _spreadOffset + weight);
            offsetDistance.Add(new Vector3(0, y, x));
        }

        return offsetDistance;
    }

    public override void Do()
    {
        base.Do();

        List<Vector3> offsetDistances = ReturnOffsetDistance(_weightApplier.StoredWeight + DisplacementWeight * bulletSpreadPowerDecreaseRatio, _nextFireCount);

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

    public override int DecreaseBullet(int ammoCountsInMagazine)
    {
        // 다음 발사할 카운트보다 현재 카운트가 적다면 현재 카운트를 넣어준다.
        if (ammoCountsInMagazine < _nextFireCount) _nextFireCount = ammoCountsInMagazine; 
        else _nextFireCount = _fireCountsInOneShoot; 
        // 아니면 그대로 진행

        return base.DecreaseBullet(ammoCountsInMagazine);
    }
}

// 칼 공격은 여기서 구현
abstract public class BaseKnifeAttack : ApplyAttack
{
    protected Timer _waitTimer; // 반대편 마우스 클릭 시 작동
    float _waitDuration; // 반대편 마우스 클릭 시 작동 기간

    public BaseKnifeAttack(Transform camTransform, float range, int targetLayer, Animator ownerAnimator, Animator weaponAnimator, bool isMainAction, string weaponName, float waitDuration, DirectionData directionData) 
        : base(camTransform, range, targetLayer, ownerAnimator, weaponAnimator, isMainAction, weaponName)
    {
        _damageConverter = new DirectionBasedDamageConverter(directionData);
        _waitTimer = new Timer();
        

        _waitDuration = waitDuration;
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

    protected abstract void PlayAnimation();
    protected void CheckRaycastHit()
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

    protected override void SpawnHitEffect(string name, Vector3 hitPosition, Vector3 hitNormal)
    {
        BaseEffect bulletHoleEffect;
        bulletHoleEffect = ObjectPooler.SpawnFromPool<BaseEffect>(name);

        bulletHoleEffect.Initialize(hitPosition, hitNormal, Quaternion.LookRotation(-hitNormal));
        bulletHoleEffect.PlayEffect();
    }

    public override void OnUpdate() 
    {
        _waitTimer.Update();
        if(_waitTimer.IsFinish)
        {
            _waitTimer.Reset();
        }
    }

    public override void OnUnLink(GameObject player) { }

    public override int DecreaseBullet(int ammoCountsInMagazine) { return ammoCountsInMagazine; }

    public override void OnReload() { }

    public override void OnLink(GameObject player) { }

    public override void OnOtherActionEventRequested()
    {
        _waitTimer.Start(_waitDuration);
    }

    public override int ReturnFireCountInOneShoot() { return 0; }


    public override void Do()
    {
        if (_waitTimer.CanStart() == false) return;
        PlayAnimation();
    }
}

public class RightKnifeAttack : BaseKnifeAttack
{
    float _delayDuration;
    Timer _delayTimer; // 공격 시 작동

    public RightKnifeAttack(Transform camTransform, float range, int targetLayer, Animator ownerAnimator, Animator weaponAnimator, bool isMainAction, string weaponName, float delayDuration, float waitDuration, DirectionData directionData)
        : base(camTransform, range, targetLayer, ownerAnimator, weaponAnimator, isMainAction, weaponName, waitDuration, directionData)
    {
        _delayDuration = delayDuration;
        _delayTimer = new Timer();
    }

    public override bool CanDo() { return true; }

    public override void CheckBulletLeftCount(int leftBulletCount) { }

    public override void Do()
    {
        base.Do();
        _delayTimer.Start(_delayDuration);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        _delayTimer.Update();
        if (_delayTimer.IsFinish)
        {
            _delayTimer.Reset();
            CheckRaycastHit();
        }
    }

    protected override void PlayAnimation() => PlayOwnerAnimation();
}

public class LeftKnifeAttack : BaseKnifeAttack
{
    float _attackLinkDuration = 2.8f;
    float _attackTime = 0;
    int _actionIndex = 0;

    float[] _delayDurations;
    Timer[] _delayTimers; // 공격 시 작동

    public LeftKnifeAttack(Transform camTransform, float range, int targetLayer, Animator ownerAnimator, Animator weaponAnimator, bool isMainAction, string weaponName, 
        float[] delayDurations, float waitDuration, float attackLinkDuration, DirectionData directionData) 
        : base(camTransform, range, targetLayer, ownerAnimator, weaponAnimator, isMainAction, weaponName, waitDuration, directionData)
    {
        _delayDurations = delayDurations;
        _attackLinkDuration = attackLinkDuration;

        _delayTimers = new Timer[_delayDurations.Length]; // 개수만큼 타이머 만들기
        for (int i = 0; i < _delayTimers.Length; i++)
        {
            _delayTimers[i] = new Timer();
        }
    }

    public override bool CanDo() { return true; }

    public override void CheckBulletLeftCount(int leftBulletCount) { }

    public override void Do()
    {
        base.Do();
        _delayTimers[_actionIndex].Start(_delayDurations[_actionIndex]);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        for (int i = 0; i < _delayTimers.Length; i++)
        {
            _delayTimers[i].Update();
            if (_delayTimers[i].IsFinish)
            {
                _delayTimers[i].Reset();
                CheckRaycastHit();
            }
        }
    }

    protected override void PlayAnimation()
    {
        if(_actionIndex == 0 && _attackTime == 0) // 첫 공격의 경우
        {
            UpdateAnimationValue();
        }
        else // 콤보 공격인 경우
        {
            if (Time.time - _attackTime < _attackLinkDuration)
            {
                UpdateAnimationValue();
                if (_actionIndex > _delayDurations.Length - 1) _actionIndex = 0;
            }
            else
            {
                // 연계 실패
                _actionIndex = 0;
                UpdateAnimationValue();
            }
        }
    }

    void UpdateAnimationValue()
    {
        PlayAnimation(_actionIndex);
        _actionIndex++;
        _attackTime = Time.time;
    }
}