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

    public abstract void TurnOffAim();

    public abstract void OnUnEquip();

    public abstract void OnUpdate();

    public abstract void OnOtherActionEventRequested();

    public abstract void TurnOffZoomWhenOtherExecute();
}

public class NoResult : ResultStrategy
{
    public override void Do() { }

    public override void OnUnLink(GameObject player) { }

    public override int DecreaseBullet(int ammoCountsInMagazine) { return ammoCountsInMagazine; }

    public override void OnReload() { }

    public override void OnUnEquip() { }

    public override void TurnOffAim() { }

    public override void OnUpdate() { }

    public override void OnOtherActionEventRequested() { }

    //public override int ReturnFireCountInOneShoot() { return 0; }

    public override void OnLink(GameObject player) { }

    public override bool CanDo() { return false; }

    public override void CheckBulletLeftCount(int leftBullet) { }

    public override void TurnOffZoomWhenOtherExecute() { }
}

public class ZoomStrategy : ResultStrategy
{
    protected GameObject _scope;
    protected bool _nowZoom;

    float _zoomDuration;
    float _scopeOnDelay;

    float _normalFieldOfView;
    protected float _zoomFieldOfView;

    Vector3 _zoomCameraPosition;

    public System.Action<GameObject, bool, float, Vector3, float, float, bool> OnZoomRequested;
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

    protected void InvokeZoomEventOtherComponent(bool isInstance)
    {
        if (_nowZoom) OnZoomRequested?.Invoke(_scope, _nowZoom, _zoomDuration, _zoomCameraPosition, _scopeOnDelay, _zoomFieldOfView, isInstance);
        else OnZoomRequested?.Invoke(_scope, _nowZoom, _zoomDuration, _zoomCameraPosition, _scopeOnDelay, _normalFieldOfView, isInstance);
    }

    protected void InvokeZoomEventOtherComponent(float fieldOfView, bool isInstance)
    {
        OnZoomRequested?.Invoke(_scope, _nowZoom, _zoomDuration, _zoomCameraPosition, _scopeOnDelay, fieldOfView, isInstance);
    }

    void Zoom(bool isInstance)
    {
        InvokeZoomEventOtherComponent(isInstance);
        OnZoomEventCall?.Invoke(_nowZoom); // 총 이벤트 호출
    }

    public override void Do() 
    {
        _nowZoom = !_nowZoom;
        Zoom(false);
    }

    protected virtual void TurnOffZoomDirectly()
    {
        if (_nowZoom == false) return;
        _nowZoom = false;
        Zoom(true);
    }

    public override void TurnOffAim() => TurnOffZoomDirectly();

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

    public override bool CanDo() { return true; }

    public override void CheckBulletLeftCount(int leftBulletCount) { }

    public override void TurnOffZoomWhenOtherExecute() { }
}

public class DoubleZoomStrategy : ZoomStrategy//, ISubject<GameObject, bool, float, float, float, float, bool>
{
    GameObject _armMesh;
    GameObject _gunMesh;
    //scope는 Find로 찾아서 사용 --> 캔버스 안에 있음

    bool nowDoubleZoom;
    float _doubleZoomFieldOfView;

    public DoubleZoomStrategy(GameObject scope, GameObject armMesh, GameObject gunMesh, Vector3 zoomCameraPosition, float zoomDuration, float scopeOnDuration, float normalFieldOfView, float zoomFieldOfView,
        float doubleZoomFieldOfView, System.Action<bool> onZoom) : base(scope, zoomCameraPosition, zoomDuration, scopeOnDuration, normalFieldOfView, zoomFieldOfView, onZoom)
    {
        _scope = scope;
        _scope.SetActive(false);

        _gunMesh = gunMesh;
        _doubleZoomFieldOfView = doubleZoomFieldOfView;
        _armMesh = armMesh;
    }

    public override void TurnOffZoomWhenOtherExecute() 
    {
        TurnOffZoomDirectly();
    }

    protected override void TurnOffZoomDirectly()
    {
        base.TurnOffZoomDirectly();
        nowDoubleZoom = false;
        ActivateMesh(true); // 메쉬 활성화
    }

    void ActivateMesh(bool nowActivate)
    {
        _gunMesh.SetActive(nowActivate);
        _armMesh.SetActive(nowActivate);
    }

    public override void Do()
    {
        if(_nowZoom == true && nowDoubleZoom == false)
        {
            ActivateMesh(false);
            // 더블 줌으로 들어감
            nowDoubleZoom = true;
            InvokeZoomEventOtherComponent(_doubleZoomFieldOfView, true);
        }
        else if(_nowZoom == true && nowDoubleZoom == true)
        {
            ActivateMesh(true);
            nowDoubleZoom = false;
            base.Do(); // 노멀로 들어감
        }
        else if (_nowZoom == false)
        {
            ActivateMesh(false);
            base.Do(); // 줌으로 들어감
        }
    }
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

    protected void PlayAnimation()
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

    protected virtual void SpawnEffect(string name, Vector3 position) { }
    protected virtual void SpawnEffect(string name, Vector3 position, Vector3 normal) { }
    protected virtual void SpawnEffect(string name, Vector3 position, Vector3 normal, bool isBlocked) { }


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
    public float DisplacementDecreaseRatio { get; set; }

    ParticleSystem _muzzleFlash;
    ParticleSystem _emptyCartridgeSpawner;
    bool _canSpawnMuzzleFlash;

    int _leftBulletCount; // 발사 전 남은 총알 체크

    public PenetrateAttack(Transform camTransform, float range, int targetLayer, int fireCountsInOneShoot, Animator ownerAnimator, Animator weaponAnimator, 
        ParticleSystem muzzleFlashSpawner, bool canSpawnMuzzleFlash, ParticleSystem emptyCartridgeSpawner, bool isMainAction, string weaponName, 
        Transform muzzle, float penetratePower, string trajectoryLineEffect, float displacementDecreaseRatio, Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> damageDictionary) 
        : base(camTransform, range, targetLayer, ownerAnimator, weaponAnimator, isMainAction, weaponName)
    {
        _muzzle = muzzle;
        _penetratePower = penetratePower;

        _trajectoryLineEffect = trajectoryLineEffect;
        _fireCountsInOneShoot = fireCountsInOneShoot;

        _muzzleFlash = muzzleFlashSpawner;
        _emptyCartridgeSpawner = emptyCartridgeSpawner;

        _canSpawnMuzzleFlash = canSpawnMuzzleFlash;

        DisplacementDecreaseRatio = displacementDecreaseRatio;

        _damageConverter = new DistanceAreaBasedDamageConverter(damageDictionary);
    }

    //public override int ReturnFireCountInOneShoot() { return _fireCountsInOneShoot; }

    public override void TurnOffAim() { }

    public override int DecreaseBullet(int ammoCountsInMagazine)
    {
        ammoCountsInMagazine -= _fireCountsInOneShoot;
        if (ammoCountsInMagazine < 0) ammoCountsInMagazine = 0;

        return ammoCountsInMagazine;
    }

    RaycastHit[] DetectCollider(Vector3 origin, Vector3 direction, float maxDistance, int layer)
    {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(origin, direction, maxDistance, layer);

        System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance)); // 맞은 객체들을 거리 기반으로 정렬해준다.

        return hits;
    }

    protected List<PenetrateData> ReturnPenetrateData(Vector3 directionOffset = default(Vector3), Vector3 positionOffset = default(Vector3))
    {
        RaycastHit[] entryHits = DetectCollider(_camTransform.position, _camTransform.forward + directionOffset, _range, _targetLayer);

        if (entryHits.Length == 0) return null; // 만약 아무도 맞지 않았다면 리턴

        Vector3 endPoint = _camTransform.position + (_camTransform.forward + directionOffset) * _range; // 레이캐스트가 닫는 가장 마지막 위치

        RaycastHit[] exitHits = DetectCollider(endPoint, -(_camTransform.forward + directionOffset), _range, _targetLayer);

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

    void DrawPenetrateLine(Vector3 hitPoint)
    {
        float diatance = Vector3.Distance(_camTransform.position, hitPoint);
        Vector3 direction = (hitPoint - _camTransform.position).normalized;

        Debug.DrawRay(_camTransform.position, direction * diatance, Color.green, 10); // 디버그 레이를 카메라가 바라보고 있는 방향으로 발사한다.
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

            SpawnEffect(effectName, penetrateData.EntryPoint, penetrateData.EntryNormal, false);
            SpawnEffect(effectName, penetrateData.ExitPoint, penetrateData.ExitNormal, false);
            // 이 경우, 진입, 퇴장 포인트 모두에 관통 이미지 추가
        }
    }

    void SpawnNonePenetrateEffect(IEffectable effectableTarget, PenetrateData penetrateData)
    {
        bool canReturnEffectName = effectableTarget.CanReturnHitEffectName(IEffectable.ConditionType.BulletNonPenetration);
        string effectName = effectableTarget.ReturnHitEffectName(IEffectable.ConditionType.BulletNonPenetration);

        if (canReturnEffectName) SpawnEffect(effectName, penetrateData.EntryPoint, penetrateData.EntryNormal, true);
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

    protected void Shoot(Vector3 directionOffset = default(Vector3), Vector3 positionOffset = default(Vector3))
    {
        List<PenetrateData> penetrateDatas = ReturnPenetrateData(directionOffset, positionOffset);
        if (penetrateDatas == null) return;

        ProgressPenetrateSequence(penetrateDatas);
    }

    protected override void SpawnEffect(string name, Vector3 hitPosition, Vector3 hitNormal, bool isBlocked)
    {
        BaseEffect bulletHoleEffect;
        bulletHoleEffect = ObjectPooler.SpawnFromPool<IEffectContainer>(name).ReturnEffect();

        bulletHoleEffect.Initialize(hitPosition, hitNormal, Quaternion.LookRotation(-hitNormal));
        bulletHoleEffect.PlayEffect();
    }

    protected override void SpawnEffect(string name, Vector3 hitPosition)
    {
        BaseEffect bulletHoleEffect;
        bulletHoleEffect = ObjectPooler.SpawnFromPool<IEffectContainer>(name).ReturnEffect();

        bulletHoleEffect.Initialize(hitPosition);
        bulletHoleEffect.PlayEffect();
    }

    void DrawTrajectoryLine(Vector3 hitPosition)
    {
        BaseEffect trajectoryLineEffect = ObjectPooler.SpawnFromPool<IEffectContainer>(_trajectoryLineEffect).ReturnEffect();
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
        PlayAnimation();
        PlayEffect();
    }

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
        DisplacementWeight = displacement * DisplacementDecreaseRatio;
    }

    public override void CheckBulletLeftCount(int leftBulletCount) { _leftBulletCount = leftBulletCount; }

    public override bool CanDo() { return _leftBulletCount > 0; }

    public override void TurnOffZoomWhenOtherExecute() { }
}

// WithWeight는 속도 백터의 길이를 받아서 적용시켜주는 인터페이스
public interface IDisplacement
{
    float DisplacementWeight { get; set; }

    float DisplacementDecreaseRatio { get; set; }

    void OnDisplacementWeightReceived(float displacement);
}

public class SingleAndExplosionScatterAttackCombination : ResultStrategy // Attack이 아니라 다른 방식으로 명명해야할 듯
{
    SingleProjectileAttack singleProjectileAttack;
    ScatterProjectileAttack scatterProjectileGunAttack;
    // 이 두 개를 생성자에서 초기화
    // 이후, 타입에 맞게 실행

    Transform _camTransform;
    int _targetLayer;

    float _findRange;
    bool _isInFront;

    public SingleAndExplosionScatterAttackCombination(Transform camTransform, float range, int targetLayer, Animator ownerAnimator, Animator weaponAnimator,
        ParticleSystem muzzleFlashSpawner, bool canSpawnMuzzleFlash, ParticleSystem emptyCartridgeSpawner, bool isMainAction, string weaponName, Transform muzzle, 
        string trajectoryLineEffect, float findRange,

        float singlePenetratePower, int bulletCountsInOneShoot, float singleBulletSpreadPowerDecreaseRatio, Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> scatterDamageDictionary,
        Vector3 frontPosition, string explosionEffectName, float scatterPenetratePower, float spreadOffset, int pelletCount, float scatterBulletSpreadPowerDecreaseRatio, Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> singleDamageDictionary)
    {
        _camTransform = camTransform;
        _targetLayer = targetLayer;
        _findRange = findRange;

        singleProjectileAttack = new SingleProjectileAttack(camTransform, range, targetLayer, ownerAnimator, weaponAnimator, muzzleFlashSpawner, canSpawnMuzzleFlash, emptyCartridgeSpawner, isMainAction,
            weaponName, muzzle, singlePenetratePower, trajectoryLineEffect, singleBulletSpreadPowerDecreaseRatio, singleDamageDictionary);

        scatterProjectileGunAttack = new ExplosionScatterProjectileAttack(camTransform, range, targetLayer, bulletCountsInOneShoot, ownerAnimator, weaponAnimator, muzzleFlashSpawner, canSpawnMuzzleFlash,
            emptyCartridgeSpawner, isMainAction, weaponName, muzzle, scatterPenetratePower, trajectoryLineEffect, spreadOffset, pelletCount, scatterBulletSpreadPowerDecreaseRatio,
            frontPosition, explosionEffectName, scatterDamageDictionary);
    }

    public override void CheckBulletLeftCount(int leftBulletCount) 
    {
        singleProjectileAttack.CheckBulletLeftCount(leftBulletCount);
        scatterProjectileGunAttack.CheckBulletLeftCount(leftBulletCount);
    }

    public override bool CanDo() { return singleProjectileAttack.CanDo() && scatterProjectileGunAttack.CanDo(); }

    protected bool IsTargetPlacedInFront()
    {
        RaycastHit hit;
        Physics.Raycast(_camTransform.position, _camTransform.forward, out hit, _findRange, _targetLayer);
        if (hit.collider == null) return false;

        return true;
    }

    public override void TurnOffZoomWhenOtherExecute() { }

    public override void Do()
    {
        _isInFront = IsTargetPlacedInFront();
        if (_isInFront)
        {
            // 상황에 따라 다르게
            singleProjectileAttack.Do();
        }
        else
        {
            scatterProjectileGunAttack.Do();
        }
    }

    public override void OnLink(GameObject player)
    {
        singleProjectileAttack.OnLink(player);
        scatterProjectileGunAttack.OnLink(player);
    }

    public override void OnUnLink(GameObject player)
    {
        singleProjectileAttack.OnUnLink(player);
        scatterProjectileGunAttack.OnUnLink(player);
    }

    public override void OnUpdate()
    {
        singleProjectileAttack.OnUpdate();
        scatterProjectileGunAttack.OnUpdate();
    }

    public override void OnOtherActionEventRequested() { }

    public override void OnReload() { }

    public override void OnUnEquip() { }

    public override void TurnOffAim() { }

    public override int DecreaseBullet(int ammoCountsInMagazine)
    {
        //bool --> 누구 공격인지 명시, 이걸로 구분해서 리턴값을 나눠줌
        if(_isInFront) return singleProjectileAttack.DecreaseBullet(ammoCountsInMagazine);
        else return scatterProjectileGunAttack.DecreaseBullet(ammoCountsInMagazine);
    }
}

    // 연사가능한 총에는 탄퍼짐이 없어야함 --> 반동으로 처리하기 때문에
public class SingleProjectileAttack : PenetrateAttack
{
    public SingleProjectileAttack(Transform camTransform, float range, int targetLayer, Animator ownerAnimator,
        Animator weaponAnimator, ParticleSystem muzzleFlashSpawner, bool canSpawnMuzzleFlash, ParticleSystem emptyCartridgeSpawner, 
        bool isMainAction, string weaponName, Transform muzzle, float penetratePower, string trajectoryLineEffect, float bulletSpreadPowerDecreaseRatio, Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> damageDictionary)
        : base(camTransform, range, targetLayer, 1, ownerAnimator, weaponAnimator, muzzleFlashSpawner, canSpawnMuzzleFlash, emptyCartridgeSpawner, 
            isMainAction, weaponName, muzzle, penetratePower, trajectoryLineEffect, bulletSpreadPowerDecreaseRatio, damageDictionary)
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
        bool isMainAction, string weaponName, Transform muzzle, float penetratePower, string trajectoryLineEffect, float bulletSpreadPowerDecreaseRatio,
        Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> damageDictionary, WeightApplier weightApplier)
        : base(camTransform, range, targetLayer, ownerAnimator, weaponAnimator, muzzleFlashSpawner, canSpawnMuzzleFlash, emptyCartridgeSpawner, 
            isMainAction, weaponName, muzzle, penetratePower, trajectoryLineEffect, bulletSpreadPowerDecreaseRatio, damageDictionary)
    {
        _weightApplier = weightApplier;
    }

    public override void Do()
    {
        DisplacementWeight += _weightApplier.StoredWeight; // 가중치 추가 적용
        base.Do();
        _weightApplier.MultiplyWeight();
    }

    public override void OnUpdate()
    {
        _weightApplier.OnUpdate();
    }
}

public class ScatterProjectileAttack : PenetrateAttack // 산탄은 가중치가 적용되지 않음
{
    float _spreadOffset;

    int _nextFireCount;
    int _pelletCount;

    protected Vector3 _frontPosition = Vector3.zero;

    public ScatterProjectileAttack(Transform camTransform, float range, int targetLayer, int bulletCountsInOneShoot, Animator ownerAnimator,
        Animator weaponAnimator, ParticleSystem muzzleFlashSpawner, bool canSpawnMuzzleFlash, ParticleSystem emptyCartridgeSpawner,
        bool isMainAction, string weaponName, Transform muzzle, float penetratePower,
       string trajectoryLineEffect, float spreadOffset, int pelletCount, float bulletSpreadPowerDecreaseRatio, Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> damageDictionary)
       : base(camTransform, range, targetLayer, bulletCountsInOneShoot, ownerAnimator, weaponAnimator, muzzleFlashSpawner, canSpawnMuzzleFlash, emptyCartridgeSpawner,
           isMainAction, weaponName, muzzle, penetratePower, trajectoryLineEffect, bulletSpreadPowerDecreaseRatio, damageDictionary)
    {
        _pelletCount = pelletCount;
        _spreadOffset = spreadOffset;
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

        List<Vector3> offsetDistances = ReturnOffsetDistance(DisplacementWeight, _pelletCount);
        for (int i = 0; i < offsetDistances.Count; i++)
        {
            Shoot(offsetDistances[i], _frontPosition);
        }
    }

    public override void OnUpdate()
    {
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

public class ExplosionScatterProjectileAttack : ScatterProjectileAttack // 산탄은 가중치가 적용되지 않음
{
    string _explosionEffectName;

    public ExplosionScatterProjectileAttack(Transform camTransform, float range, int targetLayer, int bulletCountsInOneShoot, Animator ownerAnimator,
        Animator weaponAnimator, ParticleSystem muzzleFlashSpawner, bool canSpawnMuzzleFlash, ParticleSystem emptyCartridgeSpawner,
        bool isMainAction, string weaponName, Transform muzzle, float penetratePower,
       string trajectoryLineEffect, float spreadOffset, int pelletCount, float bulletSpreadPowerDecreaseRatio, Vector3 frontPosition, string explosionEffectName, Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> damageDictionary)
       : base(camTransform, range, targetLayer, bulletCountsInOneShoot, ownerAnimator, weaponAnimator, muzzleFlashSpawner, canSpawnMuzzleFlash, emptyCartridgeSpawner,
           isMainAction, weaponName, muzzle, penetratePower, trajectoryLineEffect, spreadOffset, pelletCount, bulletSpreadPowerDecreaseRatio, damageDictionary)
    {
        _frontPosition = _camTransform.forward * frontPosition.z;
        _explosionEffectName = explosionEffectName;
    }

    public override void Do()
    {
        //여기에 이팩트 생성
        SpawnEffect(_explosionEffectName, _camTransform.position + (_camTransform.forward * _frontPosition.z));
        base.Do();
    }
}

public class ScatterProjectileAttackWithWeight : ScatterProjectileAttack
{
    WeightApplier _weightApplier;

    public ScatterProjectileAttackWithWeight(Transform camTransform, float range, int targetLayer, int bulletCountsInOneShoot, Animator ownerAnimator,
        Animator weaponAnimator, ParticleSystem muzzleFlashSpawner, bool canSpawnMuzzleFlash, ParticleSystem emptyCartridgeSpawner,
        bool isMainAction, string weaponName, Transform muzzle, float penetratePower,
       string trajectoryLineEffect, float spreadOffset, int pelletCount, float bulletSpreadPowerDecreaseRatio,  Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> damageDictionary, WeightApplier weightApplier)
       : base(camTransform, range, targetLayer, bulletCountsInOneShoot, ownerAnimator, weaponAnimator, muzzleFlashSpawner, canSpawnMuzzleFlash, emptyCartridgeSpawner, 
           isMainAction, weaponName, muzzle, penetratePower, trajectoryLineEffect, spreadOffset, pelletCount, bulletSpreadPowerDecreaseRatio, damageDictionary)
    {
        _weightApplier = weightApplier;
    }

    public override void Do()
    {
        DisplacementWeight += _weightApplier.StoredWeight;
        base.Do();
        _weightApplier.MultiplyWeight();
    }

    public override void OnUpdate()
    {
        _weightApplier.OnUpdate();
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

    protected abstract void PlayWeaponAnimation();

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
            SpawnEffect(effectName, hit.point, hit.normal);
        }
    }

    protected override void SpawnEffect(string name, Vector3 hitPosition, Vector3 hitNormal)
    {
        BaseEffect bulletHoleEffect;
        bulletHoleEffect = ObjectPooler.SpawnFromPool<IEffectContainer>(name).ReturnEffect();

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

    public override void TurnOffZoomWhenOtherExecute() { }

    public override void TurnOffAim() { }

    public override void Do()
    {
        if (_waitTimer.CanStart() == false) return;
        PlayWeaponAnimation();
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

    protected override void PlayWeaponAnimation() => PlayAnimation();
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

    protected override void PlayWeaponAnimation()
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