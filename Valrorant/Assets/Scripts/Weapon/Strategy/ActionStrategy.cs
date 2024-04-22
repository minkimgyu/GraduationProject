using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;
using System;
using Random = UnityEngine.Random;

abstract public class ActionStrategy : BaseStrategy
{

    /// <summary>
    /// Action을 호출하기 전 남은 총알 개수를 초기화 하는 함수
    /// </summary>
    //public virtual void ResetLeftBulletCount(int leftBulletCount) { }

    /// <summary>
    /// Action을 호출할 때 총알을 감소시키는 함수
    /// </summary>
    //public virtual int DecreaseBullet(int ammoCountsInMagazine) { return 0; }

    /// <summary>
    /// Action을 호출할 수 있는지 확인하는 함수
    /// </summary>
    public virtual bool CanExecute() { return true; }

    /// <summary>
    /// Action이 호출할 때 사용하는 함수
    /// </summary>
    public virtual void Execute() { }

    // 이벤트를 받는 함수는 abstract 이용해서 함수를 받으면 어떤 기능을 하는지 확실히 보여주기
    //public abstract void LinkEvent(GameObject player);

    //public abstract void UnlinkEvent(GameObject player);

    /// <summary>
    /// Reload가 호출될 경우 작동하는 함수
    /// </summary>
    //public virtual void OnReloadRequested() { }

    /// <summary>
    /// Unequip이 호출될 경우 작동하는 함수
    /// </summary>
    //public virtual void OnUnequipRequested() { }


    // 위 두 함수를 제거하고 TurnOffAim 이걸로 퉁치자

    /// <summary>
    /// Aim을 해제할 때 호출되는 함수
    /// </summary>
    public virtual void TurnOffZoomDirectly() { }

    //public abstract void Update();



    // 무슨 함수인지 확인하고 없애던가 수정하던가 해보기
    //public virtual void OnOtherActionEventRequested();

    //public virtual void TurnOffZoomWhenOtherExecute();
}

public class NoResult : ActionStrategy { }

public class BaseZoomStrategy : ActionStrategy
{
    protected float _zoomDuration;

    protected float _normalFieldOfView;
    protected float _zoomFieldOfView;

    protected Vector3 _zoomCameraPosition;

    /// <summary>
    /// bool nowTurnOn, float zoomDuration, Vector3 zoomPos, float fieldOfView
    /// </summary>
    protected Action<bool, float, Vector3, float> Zoom;

    /// <summary>
    /// 줌이 되면 Action을 바꿔줌 --> 연사 속도 등 세부 수치 변경해줘야 하기 때문에
    /// </summary>
    protected Action<bool> OnZoomRequested;

    public BaseZoomStrategy(Vector3 zoomCameraPosition, float zoomDuration, float normalFieldOfView, float zoomFieldOfView,
        Action<bool> OnZoomRequested)
    {
        _zoomDuration = zoomDuration;
        _zoomCameraPosition = zoomCameraPosition;

        _normalFieldOfView = normalFieldOfView;
        _zoomFieldOfView = zoomFieldOfView;

        this.OnZoomRequested = OnZoomRequested;
    }

    public override void LinkEvent(WeaponEventBlackboard blackboard)
    {
        Zoom = blackboard.InvokeZoom;
    }

    public override void UnlinkEvent(WeaponEventBlackboard blackboard)
    {
        TurnOffZoomDirectly();
        Zoom -= blackboard.InvokeZoom;
    }
}

public class ZoomStrategy : BaseZoomStrategy
{
    public enum State
    {
        Idle,
        Zoom
    }

    State _state;

    public ZoomStrategy(Vector3 zoomCameraPosition, float zoomDuration, float normalFieldOfView, float zoomFieldOfView,
        Action<bool> OnZoomRequested) : base(zoomCameraPosition, zoomDuration, normalFieldOfView, zoomFieldOfView, OnZoomRequested)
    {
        _state = State.Idle;
    }

    //protected void InvokeZoomEventOtherComponent(float fieldOfView)
    //{
    //    OnZoomRequested?.Invoke(_nowTurnOn, _zoomDuration, _zoomCameraPosition, fieldOfView);
    //}

    //void Zoom(bool actInstantly)
    //{
    //    if (_state == State.Idle) _state = State.Zoom;
    //    else _state = State.Idle;

    //    switch (_state)
    //    {
    //        case State.Idle:
    //            OnZoomRequested?.Invoke(_nowTurnOn, zoomDuration, _zoomCameraPosition, _normalFieldOfView);

    //            break;
    //        case State.Zoom:

    //            OnZoomRequested?.Invoke(_nowTurnOn, zoomDuration, _zoomCameraPosition, _normalFieldOfView);
    //            break;
    //    }


    //    float zoomDuration;
    //    if (actInstantly) zoomDuration = 0;
    //    else zoomDuration = _zoomDuration;

    //    if (_nowTurnOn) OnZoomRequested?.Invoke(_nowTurnOn, zoomDuration, _zoomCameraPosition, _zoomFieldOfView);
    //    else OnZoomRequested?.Invoke(_nowTurnOn, zoomDuration, _zoomCameraPosition, _normalFieldOfView);


    //    OnZoomEventCall?.Invoke(_nowTurnOn); // 총 이벤트 호출
    //}

    public override void Execute() 
    {
        //_nowTurnOn = !_nowTurnOn;
        //Zoom(false);

        if (_state == State.Idle) _state = State.Zoom;
        else _state = State.Idle;

        switch (_state)
        {
            case State.Idle:
                OnZoomRequested?.Invoke(false);
                Zoom?.Invoke(true, _zoomDuration, Vector3.zero, _normalFieldOfView);
                break;

            case State.Zoom:
                OnZoomRequested?.Invoke(true);
                Zoom?.Invoke(false, _zoomDuration, _zoomCameraPosition, _zoomFieldOfView);
                break;
        }
    }

    public override void TurnOffZoomDirectly()
    {
        //if (_nowTurnOn == false) return;
        //_nowTurnOn = false;
        //Zoom(true);

        if (_state == State.Idle) return;

        _state = State.Idle;
        OnZoomRequested?.Invoke(false);
        Zoom?.Invoke(true, 0, Vector3.zero, _normalFieldOfView);
    }

    //public override void OnReloadRequested() => TurnOffZoomDirectly();

    //public override void OnUnequipRequested() => TurnOffZoomDirectly();
}

public class DoubleZoomStrategy : BaseZoomStrategy//, ISubject<GameObject, bool, float, float, float, float, bool>
{
    //GameObject _armMesh;
    //GameObject _gunMesh;
    //scope는 Find로 찾아서 사용 --> 캔버스 안에 있음

    public enum State
    {
        Idle,
        Zoom,
        DoubleZoom
    }

    //bool nowDoubleZoom;
    float _doubleZoomFieldOfView;

    //Timer _disableMeshTimer;
    //float _disableMeshDelay;

    State _state;

    public DoubleZoomStrategy(Vector3 zoomCameraPosition, float zoomDuration, float normalFieldOfView, float zoomFieldOfView,
        float doubleZoomFieldOfView, Action<bool> OnZoomRequested) 
        : base(zoomCameraPosition, zoomDuration, normalFieldOfView, zoomFieldOfView, OnZoomRequested)
    {
        //_scope = scope;
        //_scope.SetActive(false);

        //_gunMesh = gunMesh;
        _state = State.Idle;
        _doubleZoomFieldOfView = doubleZoomFieldOfView;
        //_armMesh = armMesh;

        //_disableMeshDelay = disableMeshDelay;
        //_disableMeshTimer = new Timer();
    }

    //public override void TurnOffZoomWhenOtherExecute() 
    //{
    //    TurnOffZoomDirectly();
    //}

    //protected override void TurnOffZoomDirectly()
    //{
    //    base.TurnOffZoomDirectly();
    //    nowDoubleZoom = false;
    //    //ActivateMesh(true); // 메쉬 활성화
    //}

    //void ActivateMesh(bool nowActivate)
    //{
    //    _gunMesh.SetActive(nowActivate);
    //    _armMesh.SetActive(nowActivate);
    //}

    //public override void Update() 
    //{
    //    _disableMeshTimer.Update();

    //    if (_disableMeshTimer.IsFinish)
    //    {
    //        ActivateMesh(false);
    //        // 더블 줌으로 들어감
    //        _disableMeshTimer.Reset();
    //    }
    //}

    public override void Execute()
    {
        //_nowTurnOn = !_nowTurnOn;
        //Zoom(false);

        if (_state == State.Idle) _state = State.Zoom;
        else if(_state == State.Zoom) _state = State.DoubleZoom;
        else _state = State.Idle;

        switch (_state)
        {
            case State.Idle:
                OnZoomRequested?.Invoke(false);
                Zoom?.Invoke(false, _zoomDuration, Vector3.zero, _normalFieldOfView);
                break;

            case State.Zoom:
                OnZoomRequested?.Invoke(true);
                Zoom?.Invoke(true, _zoomDuration, _zoomCameraPosition, _zoomFieldOfView);
                break;

            case State.DoubleZoom:
                // 여기에는 넣지 않음
                Zoom?.Invoke(true, _zoomDuration, _zoomCameraPosition, _doubleZoomFieldOfView);
                break;
        }

        //if (_nowTurnOn == true && nowDoubleZoom == false)
        //{
        //    nowDoubleZoom = true;
        //    InvokeZoomEventOtherComponent(_doubleZoomFieldOfView, true);
        //}
        //else if(_nowTurnOn == true && nowDoubleZoom == true)
        //{
        //    ActivateMesh(true);
        //    nowDoubleZoom = false;
        //    base.Execute(); // 노멀로 들어감
        //}
        //else if (_nowTurnOn == false)
        //{
        //    _disableMeshTimer.Start(_disableMeshDelay);
        //    base.Execute(); // 줌으로 들어감
        //}
    }
}


abstract public class ApplyAttack : ActionStrategy
{
    /// <summary>
    /// 이벤트로 바꿔주기
    /// </summary>
    //protected Transform _camTransform;

    /// <summary>
    /// 카메라 위치를 반환해준다.
    /// </summary>
    protected Func<Vector3> ReturnRaycastPos;

    /// <summary>
    /// 카메라 방향을 반환해준다.
    /// </summary>
    protected Func<Vector3> ReturnRaycastDir;

    protected float _range;
    protected int _targetLayer;
    protected BaseDamageConverter _damageConverter;


    /// <summary>
    /// 무기의 애니메이션을 실행시킬 때 호출
    /// </summary>
    Action<string, int, float> OnPlayWeaponAnimation;

    /// <summary>
    /// 무기를 소유한 대상의 애니메이션을 실행시킬 때 호출
    /// </summary>
    protected Action<string, int, float> OnPlayOwnerAnimation;

    //bool _isMainAction;
    BaseWeapon.Name _weaponName;

    //public ApplyAttack(Type type, Transform camTransform, float range, int targetLayer, 
    //    Animator ownerAnimator, Animator weaponAnimator, BaseWeapon.Name weaponName) : base(type)
    //{
    //    _camTransform = camTransform;
    //    _range = range;
    //    _targetLayer = targetLayer;

    //    _ownerAnimator = ownerAnimator;
    //    _weaponAnimator = weaponAnimator;

    //    //_isMainAction = isMainAction;
    //    _weaponName = weaponName;
    //}

    // Func<Vector3> ReturnRaycastPos, Func<Vector3> ReturnRaycastDir, Action<string, int, float> OnPlayWeaponAnimation
    // 위 3개는 LinkEvent로 연결했다가 끊는 방식으로 사용해야한다.

    public ApplyAttack(BaseWeapon.Name weaponName, float range, int targetLayer, 
        Action<string, int, float> OnPlayWeaponAnimation)
    {
        _weaponName = weaponName;
        _range = range;
        _targetLayer = targetLayer;

        //this.ReturnRaycastPos = ReturnRaycastPos;
        //this.ReturnRaycastFowardDir = ReturnRaycastFowardDir;

        //this.OnPlayOwnerAnimation = OnPlayOwnerAnimation;
        this.OnPlayWeaponAnimation = OnPlayWeaponAnimation;


        //_camTransform = camTransform;


        //_ownerAnimator = ownerAnimator;
        //_weaponAnimator = weaponAnimator;

        //_isMainAction = isMainAction;
    }

    protected void PlayAnimation(string aniName)
    {
        OnPlayOwnerAnimation?.Invoke(_weaponName.ToString() + aniName, 0, 0);
        OnPlayWeaponAnimation?.Invoke(aniName, -1, 0);

        //_ownerAnimator.Play(_weaponName.ToString() + _type.ToString(), -1, 0);
        //_weaponAnimator.Play(_type.ToString(), -1, 0);
    }

    protected void PlayAnimation(string aniName, int index)
    {
        OnPlayOwnerAnimation?.Invoke(_weaponName.ToString() + aniName + index, -1, 0);
        OnPlayWeaponAnimation?.Invoke(aniName + index, -1, 0);

        //_ownerAnimator.Play(_weaponName.ToString() + _type.ToString() + index, -1, 0);
        //_weaponAnimator.Play(_type.ToString() + index, -1, 0);
    }


    // 데미지를 적용하는 함수는 여기에 만들어주기 ex) 총기형 데미지 함수, 칼 데미지 함수
    protected virtual float CalculateDamage(IHitable hitable, PenetrateData data, float decreaseRatio) { return default; }

    protected virtual float CalculateDamage(IHitable hitable) { return default; }

    protected virtual void ApplyDamage(IHitable hitable, RaycastHit hit) { }

    protected virtual void ApplyDamage(IHitable hitable, PenetrateData data, float decreaseRatio) { }

    protected virtual void SpawnEffect(string name, Vector3 position) { }
    protected virtual void SpawnEffect(string name, Vector3 position, Vector3 normal) { }
    protected virtual void SpawnEffect(string name, Vector3 position, Vector3 normal, bool isBlocked) { }


    //public override void OnUnequipRequested() { }

    //public override void OnReloadRequested() { }
}

abstract public class PenetrateAttack : ApplyAttack //, IDisplacement
{
    /// <summary>
    /// 머즐의 위치를 반환한다.
    /// </summary>
    Func<Vector3> ReturnMuzzlePos;

    /// <summary>
    /// 남은 총알 수를 반환한다.
    /// </summary>
    protected Func<int> ReturnLeftAmmoCount;

    /// <summary>
    /// fireCountInOnce 만큼 총알을 감소시킨다.
    /// </summary>
    Action<int> DecreaseAmmoCount;

    /// <summary>
    /// 총구 화염을 생성시킨다.
    /// </summary>
    Action SpawnMuzzleFlashEffect;

    /// <summary>
    /// 탄피를 생성시킨다.
    /// </summary>
    Action SpawnEmptyCartridge;

    //protected Transform _muzzle;

    protected float _penetratePower;

    protected float _minDecreaseRatio = 0.05f;
    protected float _trajectoryLineOffset = 1.3f;

    protected string _trajectoryLineEffect;

    /// <summary>
    /// 액션 1번에 소모되는 총알의 개수
    /// </summary>
    protected int _fireCountInOnce;

    protected float _displacementWeight = 0;
    protected float _displacementDecreaseRatio;

    public float DisplacementWeight { get; set; }

    //ParticleSystem _muzzleFlash;
    //ParticleSystem _emptyCartridgeSpawner;
    //bool _canSpawnMuzzleFlash;

    //int _leftBulletCount; // 발사 전 남은 총알 체크

    protected Action<Vector3> OnNoiseGenerateRequested;

    //public PenetrateAttack(Type type, BaseWeapon.Name weaponName, float range, int targetLayer, int fireCountsInOneShoot, Animator ownerAnimator,
    //   Animator weaponAnimator, ParticleSystem muzzleFlashSpawner, bool canSpawnMuzzleFlash, ParticleSystem emptyCartridgeSpawner,
    //   bool isMainAction, string weaponName, Transform muzzle, float penetratePower, string trajectoryLineEffect, float displacementDecreaseRatio,
    //   Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> damageDictionary, Action<Vector3> OnNoiseGenerateRequested)
    //   : base(camTransform, range, targetLayer, ownerAnimator, weaponAnimator, isMainAction, weaponName)
    //{
    //    _muzzle = muzzle;
    //    _penetratePower = penetratePower;

    //    _trajectoryLineEffect = trajectoryLineEffect;
    //    _fireCountsInOneShoot = fireCountsInOneShoot;

    //    _muzzleFlash = muzzleFlashSpawner;
    //    _emptyCartridgeSpawner = emptyCartridgeSpawner;

    //    _canSpawnMuzzleFlash = canSpawnMuzzleFlash;

    //    _displacementDecreaseRatio = displacementDecreaseRatio;

    //    _damageConverter = new DistanceAreaBasedDamageConverter(damageDictionary);

    //    this.OnNoiseGenerateRequested = OnNoiseGenerateRequested;
    //}

    // trajectoryLineEffect --> 이거는 일단 통합해서 쓰다가 나중에 추가하는 걸로 하자
    public PenetrateAttack(BaseWeapon.Name weaponName, float range, int targetLayer, int fireCountInOnce,
        float penetratePower, float displacementDecreaseRatio, Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> damageDictionary,

        Action<string, int, float> OnPlayWeaponAnimation, Func<Vector3> ReturnMuzzlePos, Func<int> ReturnLeftAmmoCount,
        Action<int> DecreaseAmmoCount, Action SpawnMuzzleFlashEffect, Action SpawnEmptyCartridge, Action<Vector3> OnNoiseGenerateRequested)

        : base(weaponName, range, targetLayer, OnPlayWeaponAnimation)
    {
        _fireCountInOnce = fireCountInOnce;
        _penetratePower = penetratePower;
        _displacementDecreaseRatio = displacementDecreaseRatio;
        _damageConverter = new DistanceAreaBasedDamageConverter(damageDictionary);
        _trajectoryLineEffect = "TrajectoryLine";


        this.ReturnMuzzlePos = ReturnMuzzlePos;
        this.ReturnLeftAmmoCount = ReturnLeftAmmoCount;
        this.DecreaseAmmoCount = DecreaseAmmoCount;

        this.SpawnMuzzleFlashEffect = SpawnMuzzleFlashEffect;
        this.SpawnEmptyCartridge = SpawnEmptyCartridge;
        this.OnNoiseGenerateRequested = OnNoiseGenerateRequested;
    }

    //public override int ReturnFireCountInOneShoot() { return _fireCountsInOneShoot; }

    //public override void TurnOffZoomDirectly() { }

    /// <summary>
    /// 이 부분은 총기 스크립트로 넘겨주기
    /// </summary>
    //public override int DecreaseBullet(int ammoCountsInMagazine)
    //{
    //    ammoCountsInMagazine -= _fireCountInOnce;
    //    if (ammoCountsInMagazine < 0) ammoCountsInMagazine = 0;

    //    return ammoCountsInMagazine;
    //}

    RaycastHit[] DetectCollider(Vector3 origin, Vector3 direction, float maxDistance, int layer)
    {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(origin, direction, maxDistance, layer);

        Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance)); // 맞은 객체들을 거리 기반으로 정렬해준다.

        return hits;
    }

    protected List<PenetrateData> ReturnPenetrateData(Vector3 directionOffset = default(Vector3), Vector3 positionOffset = default(Vector3))
    {
        Vector3 camPos = Vector3.zero;
        Vector3 camFowardDir = Vector3.zero;

        camPos = ReturnRaycastPos();
        camFowardDir = ReturnRaycastDir();

        RaycastHit[] entryHits = DetectCollider(camPos, camFowardDir + directionOffset, _range, _targetLayer);

        if (entryHits.Length == 0) return null; // 만약 아무도 맞지 않았다면 리턴

        Vector3 endPoint = camPos + (camFowardDir + directionOffset) * _range; // 레이캐스트가 닫는 가장 마지막 위치

        RaycastHit[] exitHits = DetectCollider(endPoint, -(camFowardDir + directionOffset), _range, _targetLayer);

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

                    float distanceFromStartPoint = Vector3.Distance(camPos, entryHits[i].point);

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
        Vector3 camPos = Vector3.zero;
        camPos = ReturnRaycastPos();

        float diatance = Vector3.Distance(camPos, hitPoint);
        Vector3 direction = (hitPoint - camPos).normalized;

        Debug.DrawRay(camPos, direction * diatance, Color.green, 10); // 디버그 레이를 카메라가 바라보고 있는 방향으로 발사한다.
    }

    void DrawPenetrateLineUntilExitPoint(PenetrateData penetrateData)
    {
        Vector3 muzzlePos = Vector3.zero;
        muzzlePos = ReturnMuzzlePos();

        Vector3 offsetDir = (penetrateData.ExitPoint - muzzlePos).normalized * _trajectoryLineOffset;
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
        bool canReturnEffectName = effectableTarget.CanReturnHitEffectName(IEffectable.ConditionType.Penetration);
        if (canReturnEffectName)
        {
            string effectName = effectableTarget.ReturnHitEffectName(IEffectable.ConditionType.Penetration);

            SpawnEffect(effectName, penetrateData.EntryPoint, penetrateData.EntryNormal, false);
            SpawnEffect(effectName, penetrateData.ExitPoint, penetrateData.ExitNormal, false);
            // 이 경우, 진입, 퇴장 포인트 모두에 관통 이미지 추가
        }
    }

    void SpawnNonePenetrateEffect(IEffectable effectableTarget, PenetrateData penetrateData)
    {
        bool canReturnEffectName = effectableTarget.CanReturnHitEffectName(IEffectable.ConditionType.NonPenetration);
        string effectName = effectableTarget.ReturnHitEffectName(IEffectable.ConditionType.NonPenetration);

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
        bulletHoleEffect = ObjectPooler.SpawnFromPool<BaseEffect>(name);

        bulletHoleEffect.Initialize(hitPosition, hitNormal, Quaternion.LookRotation(-hitNormal));
        bulletHoleEffect.PlayEffect();
    }

    protected override void SpawnEffect(string name, Vector3 hitPosition)
    {
        BaseEffect bulletHoleEffect;
        bulletHoleEffect = ObjectPooler.SpawnFromPool<BaseEffect>(name);

        bulletHoleEffect.Initialize(hitPosition);
        bulletHoleEffect.PlayEffect();
    }

    void DrawTrajectoryLine(Vector3 hitPosition)
    {
        Vector3 muzzlePos = ReturnMuzzlePos();

        BaseEffect trajectoryLineEffect = ObjectPooler.SpawnFromPool<BaseEffect>(_trajectoryLineEffect);
        trajectoryLineEffect.Initialize(hitPosition, muzzlePos);
        trajectoryLineEffect.PlayEffect();
    }

    void PlayEffect()
    {
        SpawnMuzzleFlashEffect?.Invoke();
        SpawnEmptyCartridge?.Invoke();
    }

    //public override void OnOtherActionEventRequested() { }

    public override void Execute()
    {
        DecreaseAmmoCount?.Invoke(_fireCountInOnce); // 총알 감소

        Vector3 muzzlePos = ReturnMuzzlePos();
        // _muzzle 사용
        OnNoiseGenerateRequested?.Invoke(muzzlePos);

        // 여기에 사운드 발생 기능 추가
        PlayAnimation("Fire");
        PlayEffect();
    }

    public override void UnlinkEvent(WeaponEventBlackboard blackboard)
    {
        ReturnRaycastPos -= blackboard.ReturnRaycastPos;
        ReturnRaycastDir -= blackboard.ReturnRaycastDir;
        OnPlayOwnerAnimation -= blackboard.OnPlayOwnerAnimation;

        blackboard.OnDisplacementRequested -= OnDisplacementWeightReceived;
    }

    public override void LinkEvent(WeaponEventBlackboard blackboard)
    {
        ReturnRaycastPos += blackboard.ReturnRaycastPos;
        ReturnRaycastDir += blackboard.ReturnRaycastDir;
        OnPlayOwnerAnimation += blackboard.OnPlayOwnerAnimation;

        blackboard.OnDisplacementRequested += OnDisplacementWeightReceived;
    }

    public void OnDisplacementWeightReceived(float displacement)
    {
        _displacementWeight = displacement * _displacementDecreaseRatio;
    }

    //public override void ResetLeftBulletCount(int leftBulletCount) { _leftBulletCount = leftBulletCount; }

    public override bool CanExecute() { return ReturnLeftAmmoCount() > 0; } // _leftBulletCount > 0

    //public override void TurnOffZoomWhenOtherExecute() { }
}

// WithWeight는 속도 백터의 길이를 받아서 적용시켜주는 인터페이스
//public interface IDisplacement
//{
//    //float DisplacementWeight { get; set; }

//    //float DisplacementDecreaseRatio { get; set; }

//    void OnDisplacementWeightReceived(float displacement);
//}

// 연사가능한 총에는 탄퍼짐이 없어야함 --> 반동으로 처리하기 때문에
public class SingleProjectileAttack : PenetrateAttack
{
    public SingleProjectileAttack(BaseWeapon.Name weaponName, float range, int targetLayer, int fireCountInOnce,
        float penetratePower, float displacementDecreaseRatio, Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> damageDictionary,

        Action<string, int, float> OnPlayWeaponAnimation, Func<Vector3> ReturnMuzzlePos, Func<int> ReturnLeftAmmoCount,
        Action<int> DecreaseAmmoCount, Action SpawnMuzzleFlashEffect, Action SpawnEmptyCartridge, Action<Vector3> OnNoiseGenerateRequested)

        : base(weaponName, range, targetLayer, fireCountInOnce, penetratePower, displacementDecreaseRatio, 
            damageDictionary, OnPlayWeaponAnimation, ReturnMuzzlePos, ReturnLeftAmmoCount,
            DecreaseAmmoCount, SpawnMuzzleFlashEffect, SpawnEmptyCartridge, OnNoiseGenerateRequested)
    {
    }

    //public SingleProjectileAttack(Transform camTransform, float range, int targetLayer, Animator ownerAnimator,
    //    Animator weaponAnimator, ParticleSystem muzzleFlashSpawner, bool canSpawnMuzzleFlash, ParticleSystem emptyCartridgeSpawner,
    //    bool isMainAction, string weaponName, Transform muzzle, float penetratePower, string trajectoryLineEffect, float bulletSpreadPowerDecreaseRatio,
    //    Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> damageDictionary, Action<Vector3> OnNoiseGenerateRequested)
    //    : base(camTransform, range, targetLayer, 1, ownerAnimator, weaponAnimator, muzzleFlashSpawner, canSpawnMuzzleFlash, emptyCartridgeSpawner,
    //        isMainAction, weaponName, muzzle, penetratePower, trajectoryLineEffect, bulletSpreadPowerDecreaseRatio, damageDictionary, OnNoiseGenerateRequested)
    //{
    //}

    protected Vector3 ReturnOffset(float weight)
    {
        float x = Random.Range(-weight, weight);
        float y = Random.Range(-weight, weight);

        return new Vector3(0, y, x);
    }

    public override void Execute()
    {
        base.Execute();
        Vector3 multifliedOffset = ReturnOffset(_displacementWeight);
        Shoot(multifliedOffset);
    }

    //public override void Update()
    //{
    //}
}

// 점사 가능한 총에는 탄 퍼짐 추가

// WithWeight는 탄 퍼짐을 의미함
public class SingleProjectileAttackWithWeight : SingleProjectileAttack // 가중치가 적용되는 공격
{
    WeightApplier _weightApplier;

    //public SingleProjectileAttackWithWeight(Transform camTransform, float range, int targetLayer, Animator ownerAnimator,
    //    Animator weaponAnimator, ParticleSystem muzzleFlashSpawner, bool canSpawnMuzzleFlash, ParticleSystem emptyCartridgeSpawner,
    //    bool isMainAction, string weaponName, Transform muzzle, float penetratePower, string trajectoryLineEffect, float bulletSpreadPowerDecreaseRatio,
    //    Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> damageDictionary, Action<Vector3> OnNoiseGenerateRequested, WeightApplier weightApplier)
    //    : base(camTransform, range, targetLayer, ownerAnimator, weaponAnimator, muzzleFlashSpawner, canSpawnMuzzleFlash, emptyCartridgeSpawner,
    //        isMainAction, weaponName, muzzle, penetratePower, trajectoryLineEffect, bulletSpreadPowerDecreaseRatio, damageDictionary, OnNoiseGenerateRequested)
    //{
    //    _weightApplier = weightApplier;
    //}

    public SingleProjectileAttackWithWeight(BaseWeapon.Name weaponName, float range, int targetLayer, int fireCountInOnce,
        float penetratePower, float displacementDecreaseRatio, WeightApplier weightApplier, Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> damageDictionary,
        

        Action<string, int, float> OnPlayWeaponAnimation, Func<Vector3> ReturnMuzzlePos, Func<int> ReturnLeftAmmoCount,
        Action<int> DecreaseAmmoCount, Action SpawnMuzzleFlashEffect, Action SpawnEmptyCartridge, Action<Vector3> OnNoiseGenerateRequested)

        : base(weaponName, range, targetLayer, fireCountInOnce, penetratePower, displacementDecreaseRatio, 
            damageDictionary, OnPlayWeaponAnimation, ReturnMuzzlePos, ReturnLeftAmmoCount,
            DecreaseAmmoCount, SpawnMuzzleFlashEffect, SpawnEmptyCartridge, OnNoiseGenerateRequested)
    {
        _weightApplier = weightApplier;
    }

    public override void Execute()
    {
        _displacementWeight += _weightApplier.StoredWeight; // 가중치 추가 적용
        base.Execute();
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

    int _storedFireCount;
    int _pelletCount;

    protected Vector3 _frontPosition = Vector3.zero;

    //public ScatterProjectileAttack(Transform camTransform, float range, int targetLayer, int bulletCountsInOneShoot, Animator ownerAnimator,
    //    Animator weaponAnimator, ParticleSystem muzzleFlashSpawner, bool canSpawnMuzzleFlash, ParticleSystem emptyCartridgeSpawner,
    //    bool isMainAction, string weaponName, Transform muzzle, float penetratePower,
    //   string trajectoryLineEffect, float spreadOffset, int pelletCount, float bulletSpreadPowerDecreaseRatio, Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> damageDictionary,
    //   Action<Vector3> OnNoiseGenerateRequested)
    //   : base(camTransform, range, targetLayer, bulletCountsInOneShoot, ownerAnimator, weaponAnimator, muzzleFlashSpawner, canSpawnMuzzleFlash, emptyCartridgeSpawner,
    //       isMainAction, weaponName, muzzle, penetratePower, trajectoryLineEffect, bulletSpreadPowerDecreaseRatio, damageDictionary, OnNoiseGenerateRequested)
    //{
    //    _pelletCount = pelletCount;
    //    _spreadOffset = spreadOffset;
    //    _nextFireCount = _fireCountInOnce; // 초기 발사 카운트는 _fireCountsInOneShoot를 대입해준다.

    //    this.OnNoiseGenerateRequested = OnNoiseGenerateRequested;
    //}

    public ScatterProjectileAttack(BaseWeapon.Name weaponName, float range, int targetLayer, int fireCountInOnce,
        float penetratePower, float displacementDecreaseRatio, int pelletCount, float spreadOffset, //int nextFireCount,
        Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> damageDictionary,

        Action<string, int, float> OnPlayWeaponAnimation, Func<Vector3> ReturnMuzzlePos, Func<int> ReturnLeftAmmoCount,
        Action<int> DecreaseAmmoCount, Action SpawnMuzzleFlashEffect, Action SpawnEmptyCartridge, Action<Vector3> OnNoiseGenerateRequested)

        : base(weaponName, range, targetLayer, fireCountInOnce, penetratePower, displacementDecreaseRatio, 
            damageDictionary, OnPlayWeaponAnimation, ReturnMuzzlePos, ReturnLeftAmmoCount,
            DecreaseAmmoCount, SpawnMuzzleFlashEffect, SpawnEmptyCartridge, OnNoiseGenerateRequested)
    {
        _pelletCount = pelletCount;
        _spreadOffset = spreadOffset;
        _storedFireCount = _fireCountInOnce; // _storedFireCount에 저장해둔다.
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

    public override void Execute()
    {
        int leftAmmoCount = ReturnLeftAmmoCount();
        if (_storedFireCount > leftAmmoCount) _fireCountInOnce = leftAmmoCount; //  _fireCountInOnce 재지정

        base.Execute();

        List<Vector3> offsetDistances = ReturnOffsetDistance(_displacementWeight, _pelletCount);
        for (int i = 0; i < offsetDistances.Count; i++)
        {
            Shoot(offsetDistances[i], _frontPosition);
        }
    }

    //public override int DecreaseBullet(int ammoCountsInMagazine)
    //{
    //    // 다음 발사할 카운트보다 현재 카운트가 적다면 현재 카운트를 넣어준다.
    //    if (ammoCountsInMagazine < _storedFireCount) _storedFireCount = ammoCountsInMagazine;
    //    else _storedFireCount = _fireCountInOnce;
    //    // 아니면 그대로 진행

    //    return base.DecreaseBullet(ammoCountsInMagazine);
    //}
}

public class ScatterProjectileAttackWithWeight : ScatterProjectileAttack
{
    WeightApplier _weightApplier;

    //public ScatterProjectileAttackWithWeight(Transform camTransform, float range, int targetLayer, int bulletCountsInOneShoot, Animator ownerAnimator,
    //    Animator weaponAnimator, ParticleSystem muzzleFlashSpawner, bool canSpawnMuzzleFlash, ParticleSystem emptyCartridgeSpawner,
    //    bool isMainAction, string weaponName, Transform muzzle, float penetratePower,
    //   string trajectoryLineEffect, float spreadOffset, int pelletCount, float bulletSpreadPowerDecreaseRatio,
    //   Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> damageDictionary, Action<Vector3> OnNoiseGenerateRequested, WeightApplier weightApplier)
    //   : base(camTransform, range, targetLayer, bulletCountsInOneShoot, ownerAnimator, weaponAnimator, muzzleFlashSpawner, canSpawnMuzzleFlash, emptyCartridgeSpawner,
    //       isMainAction, weaponName, muzzle, penetratePower, trajectoryLineEffect, spreadOffset, pelletCount, bulletSpreadPowerDecreaseRatio, damageDictionary, OnNoiseGenerateRequested)
    //{
    //    _weightApplier = weightApplier;
    //}

    public ScatterProjectileAttackWithWeight(BaseWeapon.Name weaponName, float range, int targetLayer, int fireCountInOnce,
        float penetratePower, float displacementDecreaseRatio, int pelletCount, float spreadOffset, WeightApplier weightApplier, 
        Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> damageDictionary,

        Action<string, int, float> OnPlayWeaponAnimation, Func<Vector3> ReturnMuzzlePos, Func<int> ReturnLeftAmmoCount,
        Action<int> DecreaseAmmoCount, Action SpawnMuzzleFlashEffect, Action SpawnEmptyCartridge, Action<Vector3> OnNoiseGenerateRequested)

        : base(weaponName, range, targetLayer, fireCountInOnce, penetratePower, displacementDecreaseRatio, pelletCount, 
            spreadOffset, damageDictionary, OnPlayWeaponAnimation, ReturnMuzzlePos, ReturnLeftAmmoCount,
            DecreaseAmmoCount, SpawnMuzzleFlashEffect, SpawnEmptyCartridge, OnNoiseGenerateRequested)
    {
        _weightApplier = weightApplier;
    }

    public override void Execute()
    {
        _displacementWeight += _weightApplier.StoredWeight;
        base.Execute();
        _weightApplier.MultiplyWeight();
    }

    public override void OnUpdate()
    {
        _weightApplier.OnUpdate();
    }
}

public class ExplosionScatterProjectileAttack : ScatterProjectileAttack // 산탄은 가중치가 적용되지 않음
{
    string _explosionEffectName;

    //public ExplosionScatterProjectileAttack(Transform camTransform, float range, int targetLayer, int bulletCountsInOneShoot, Animator ownerAnimator,
    //    Animator weaponAnimator, ParticleSystem muzzleFlashSpawner, bool canSpawnMuzzleFlash, ParticleSystem emptyCartridgeSpawner,
    //    bool isMainAction, string weaponName, Transform muzzle, float penetratePower,
    //   string trajectoryLineEffect, float spreadOffset, int pelletCount, float bulletSpreadPowerDecreaseRatio, Vector3 frontPosition, string explosionEffectName,
    //   Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> damageDictionary, Action<Vector3> OnNoiseGenerateRequested)
    //   : base(camTransform, range, targetLayer, bulletCountsInOneShoot, ownerAnimator, weaponAnimator, muzzleFlashSpawner, canSpawnMuzzleFlash, emptyCartridgeSpawner,
    //       isMainAction, weaponName, muzzle, penetratePower, trajectoryLineEffect, spreadOffset, pelletCount, bulletSpreadPowerDecreaseRatio, damageDictionary, OnNoiseGenerateRequested)
    //{
    //    _frontPosition = _camTransform.forward * frontPosition.z;
    //    _explosionEffectName = explosionEffectName;
    //}

    float _frontDistance;

    public ExplosionScatterProjectileAttack(BaseWeapon.Name weaponName, float range, int targetLayer, int fireCountInOnce,
        float penetratePower, float displacementDecreaseRatio, int pelletCount, float spreadOffset, float frontDistance,
        string explosionEffectName, Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> damageDictionary,

        Action<string, int, float> OnPlayWeaponAnimation, Func<Vector3> ReturnMuzzlePos, Func<int> ReturnLeftAmmoCount,
        Action<int> DecreaseAmmoCount, Action SpawnMuzzleFlashEffect, Action SpawnEmptyCartridge, Action<Vector3> OnNoiseGenerateRequested)

        : base(weaponName, range, targetLayer, fireCountInOnce, penetratePower, displacementDecreaseRatio, pelletCount, spreadOffset,
            damageDictionary, OnPlayWeaponAnimation, ReturnMuzzlePos, ReturnLeftAmmoCount,
            DecreaseAmmoCount, SpawnMuzzleFlashEffect, SpawnEmptyCartridge, OnNoiseGenerateRequested)
    {
        _frontDistance = frontDistance;
        _explosionEffectName = explosionEffectName;
    }

    public override void Execute()
    {
        Vector3 camFowardDir = ReturnRaycastDir();
        Vector3 camPos = ReturnRaycastPos();

        _frontPosition = ReturnRaycastDir() * _frontDistance;

        // 총기 앞에 이팩트 생성
        SpawnEffect(_explosionEffectName, camPos + (camFowardDir * _frontPosition.z));
        base.Execute();
    }
}

public class SingleAndExplosionScatterAttackCombination : ActionStrategy // Attack이 아니라 다른 방식으로 명명해야할 듯
{
    SingleProjectileAttack singleProjectileAttack;
    ScatterProjectileAttack scatterProjectileGunAttack;
    // 이 두 개를 생성자에서 초기화
    // 이후, 타입에 맞게 실행

    Func<Vector3> ReturnRaycastPos;
    Func<Vector3> ReturnRaycastDir;

    int _targetLayer;

    float _findRange;
    bool _isInFront;

    public SingleAndExplosionScatterAttackCombination(

        BaseWeapon.Name weaponName, float range, int targetLayer,

        int singleBulletCountsInOneShoot, float singlePenetratePower, float singleDisplacementDecreaseRatio,
        int scatterBulletCountsInOneShoot, float scatterPenetratePower, float scatterDisplacementDecreaseRatio, int pelletCount, float spreadOffset, 

        float frontDistance, string explosionEffectName,

        Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> damageDictionary,
        Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> scatterDamageDictionary, float findRange,


        Action<string, int, float> OnPlayWeaponAnimation, Func<Vector3> ReturnMuzzlePos, Func<int> ReturnLeftAmmoCount,
        Action<int> DecreaseAmmoCount, Action SpawnMuzzleFlashEffect, Action SpawnEmptyCartridge, Action<Vector3> OnNoiseGenerateRequested
        )
    {
        _targetLayer = targetLayer;
        _findRange = findRange;

        singleProjectileAttack = new SingleProjectileAttack(weaponName, range, targetLayer, singleBulletCountsInOneShoot, singlePenetratePower, singleDisplacementDecreaseRatio,
            damageDictionary, OnPlayWeaponAnimation, ReturnMuzzlePos, ReturnLeftAmmoCount, DecreaseAmmoCount, 
            SpawnMuzzleFlashEffect, SpawnEmptyCartridge, OnNoiseGenerateRequested);

        scatterProjectileGunAttack = new ExplosionScatterProjectileAttack(weaponName, range, targetLayer, scatterBulletCountsInOneShoot, scatterPenetratePower, scatterDisplacementDecreaseRatio,
            pelletCount, spreadOffset, frontDistance, explosionEffectName, scatterDamageDictionary, OnPlayWeaponAnimation, ReturnMuzzlePos, ReturnLeftAmmoCount, DecreaseAmmoCount,
            SpawnMuzzleFlashEffect, SpawnEmptyCartridge, OnNoiseGenerateRequested);
    }

    //public override void ResetLeftBulletCount(int leftBulletCount) 
    //{
    //    singleProjectileAttack.ResetLeftBulletCount(leftBulletCount);
    //    scatterProjectileGunAttack.ResetLeftBulletCount(leftBulletCount);
    //}

    public override bool CanExecute() { return singleProjectileAttack.CanExecute() && scatterProjectileGunAttack.CanExecute(); }

    protected bool IsTargetPlacedInFront()
    {
        Vector3 camPos = ReturnRaycastPos();
        Vector3 camFowardDir = ReturnRaycastDir();

        RaycastHit hit;
        Physics.Raycast(camPos, camFowardDir, out hit, _findRange, _targetLayer);
        if (hit.collider == null) return false;

        return true;
    }

    //public override void TurnOffZoomWhenOtherExecute() { }

    public override void Execute()
    {
        _isInFront = IsTargetPlacedInFront();
        if (_isInFront) singleProjectileAttack.Execute();
        else scatterProjectileGunAttack.Execute();
    }

    public override void LinkEvent(WeaponEventBlackboard blackboard)
    {
        ReturnRaycastPos = blackboard.ReturnRaycastPos;
        ReturnRaycastDir = blackboard.ReturnRaycastDir;

        singleProjectileAttack.LinkEvent(blackboard);
        scatterProjectileGunAttack.LinkEvent(blackboard);
    }

    public override void UnlinkEvent(WeaponEventBlackboard blackboard)
    {
        ReturnRaycastPos -= blackboard.ReturnRaycastPos;
        ReturnRaycastDir -= blackboard.ReturnRaycastDir;

        singleProjectileAttack.UnlinkEvent(blackboard);
        scatterProjectileGunAttack.UnlinkEvent(blackboard);
    }
}

// 칼 공격은 여기서 구현
abstract public class BaseKnifeAttack : ApplyAttack
{
    float _delayForNextStab;
    StopwatchTimer _delayTimer; // 공격 시 작동

    public BaseKnifeAttack(BaseWeapon.Name weaponName, float range, int targetLayer, float delaysForNextStab, DirectionData directionData,
        
        Action<string, int, float> OnPlayWeaponAnimation) 
        : base(weaponName, range, targetLayer, OnPlayWeaponAnimation)
    {
        _damageConverter = new DirectionBasedDamageConverter(directionData);

        _delayForNextStab = delaysForNextStab;
        _delayTimer = new StopwatchTimer();
    }

    public override void UnlinkEvent(WeaponEventBlackboard blackboard)
    {
        ReturnRaycastPos -= blackboard.ReturnRaycastPos;
        ReturnRaycastDir -= blackboard.ReturnRaycastDir;
        OnPlayOwnerAnimation -= blackboard.OnPlayOwnerAnimation;
    }

    public override void LinkEvent(WeaponEventBlackboard blackboard)
    {
        ReturnRaycastPos += blackboard.ReturnRaycastPos;
        ReturnRaycastDir += blackboard.ReturnRaycastDir;
        OnPlayOwnerAnimation += blackboard.OnPlayOwnerAnimation;
    }

    protected override float CalculateDamage(IHitable hitable)
    {
        Vector3 camFowardDir = ReturnRaycastDir();
        return _damageConverter.ReturnDamage(camFowardDir, hitable.IDamage.GetFowardVector());
    }

    protected override void ApplyDamage(IHitable hitable, RaycastHit hit)
    {
        float damage = CalculateDamage(hitable);
        hitable.OnHit(damage, hit.point, hit.normal);
    }

    protected abstract void PlayKnifeAnimation();

    protected void Stab()
    {
        Vector3 camPos = ReturnRaycastPos();
        Vector3 camFowardDir = ReturnRaycastDir();

        RaycastHit hit;
        Physics.Raycast(camPos, camFowardDir, out hit, _range, _targetLayer);
        if (hit.collider == null) return;

        IEffectable effectable;
        hit.collider.TryGetComponent(out effectable);
        if (effectable == null) return;

        bool canReturnEffectName = effectable.CanReturnHitEffectName(IEffectable.ConditionType.Stabbing);
        if (canReturnEffectName)
        {
            string effectName = effectable.ReturnHitEffectName(IEffectable.ConditionType.Stabbing);
            SpawnEffect(effectName, hit.point, hit.normal);
        }

        IHitable hitable;
        hit.collider.TryGetComponent(out hitable);
        if (hitable == null) return;

        ApplyDamage(hitable, hit);
    }

    protected override void SpawnEffect(string name, Vector3 hitPosition, Vector3 hitNormal)
    {
        BaseEffect effect = ObjectPooler.SpawnFromPool<BaseEffect>(name);

        effect.Initialize(hitPosition, hitNormal, Quaternion.LookRotation(-hitNormal));
        effect.PlayEffect();
    }

    public override void OnUpdate()
    {
        if (_delayTimer.CurrentState != StopwatchTimer.State.Finish) return;

        Stab();
        _delayTimer.Reset();
    }

    public override void Execute()
    {
        PlayKnifeAnimation();
        _delayTimer.Start(_delayForNextStab);
    }
}

public class RightKnifeAttack : BaseKnifeAttack
{
    public RightKnifeAttack(BaseWeapon.Name weaponName, float range, int targetLayer, 
        float delayForNextStab, DirectionData directionData,

        Action<string, int, float> OnPlayWeaponAnimation)
        : base(weaponName, range, targetLayer, delayForNextStab, directionData, OnPlayWeaponAnimation)
    {
    }

    protected override void PlayKnifeAnimation() => PlayAnimation("Stab");
}

public class LeftKnifeAttack : BaseKnifeAttack
{
    float _stabLinkDuration = 2.8f;
    int _stabIndex = 0;

    StopwatchTimer _stabLinkTimer;

    int _animationCount;

    public LeftKnifeAttack(BaseWeapon.Name weaponName, float range, int targetLayer,
        int animationCnt, float delayForNextStab, float attackLinkDuration, DirectionData directionData,

        Action<string, int, float> OnPlayWeaponAnimation)
        : base(weaponName, range, targetLayer, delayForNextStab, directionData, OnPlayWeaponAnimation)
    {
        _stabLinkDuration = attackLinkDuration;

        _animationCount = animationCnt;

        _stabLinkTimer = new StopwatchTimer();
    }

    public override void Execute()
    {
        base.Execute();
        _stabLinkTimer.Start(_stabLinkDuration);
    }

    protected override void PlayKnifeAnimation()
    {
        if(_stabLinkTimer.CurrentState == StopwatchTimer.State.Running)
        {
            _stabIndex++;
            if (_stabIndex > _animationCount - 1) _stabIndex = 0;
        }
        else
        {
            _stabIndex = 0;
        }

        _stabLinkTimer.Reset();

        PlayAnimation("Stab", _stabIndex);
    }
}