using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;
using System;
using Random = UnityEngine.Random;

abstract public class ActionStrategy : BaseStrategy
{
    public ActionStrategy(Action<SoundType, bool> PlaySound)
    {
        this.PlaySound = PlaySound;
    }

    protected Action<SoundType, bool> PlaySound;

    /// <summary>
    /// Action을 호출할 수 있는지 확인하는 함수
    /// </summary>
    public virtual bool CanExecute() { return true; }

    /// <summary>
    /// Action이 호출할 때 사용하는 함수
    /// </summary>
    public virtual void Execute() { }

    /// <summary>
    /// Aim을 해제할 때 호출되는 함수
    /// </summary>
    public virtual void TurnOffZoomDirectly() { }
}

public class NoAction : ActionStrategy
{
    public NoAction(Action<SoundType, bool> PlaySound) : base(PlaySound)
    {
    }
}

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
        Action<bool> OnZoomRequested, Action<SoundType, bool> PlaySound) : base(PlaySound)
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
        Action<bool> OnZoomRequested, Action<SoundType, bool> PlaySound) : base(zoomCameraPosition, zoomDuration, normalFieldOfView, zoomFieldOfView, OnZoomRequested, PlaySound)
    {
        _state = State.Idle;
    }

    public override void Execute() 
    {
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
        if (_state == State.Idle) return;

        _state = State.Idle;
        OnZoomRequested?.Invoke(false);
        Zoom?.Invoke(true, 0, Vector3.zero, _normalFieldOfView);
    }
}

public class DoubleZoomStrategy : BaseZoomStrategy
{
    public enum State
    {
        Idle,
        Zoom,
        DoubleZoom
    }

    float _doubleZoomFieldOfView;
    State _state;

    public DoubleZoomStrategy(Vector3 zoomCameraPosition, float zoomDuration, float normalFieldOfView, float zoomFieldOfView,
        float doubleZoomFieldOfView, Action<bool> OnZoomRequested, Action<SoundType, bool> PlaySound) 
        : base(zoomCameraPosition, zoomDuration, normalFieldOfView, zoomFieldOfView, OnZoomRequested, PlaySound)
    {
        _state = State.Idle;
        _doubleZoomFieldOfView = doubleZoomFieldOfView;
    }
    

    public override void Execute()
    {
        if (_state == State.Idle) _state = State.Zoom;
        else if(_state == State.Zoom) _state = State.DoubleZoom;
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

            case State.DoubleZoom:
                // 여기에는 넣지 않음
                Zoom?.Invoke(false, _zoomDuration, _zoomCameraPosition, _doubleZoomFieldOfView);
                break;
        }
    }

    public override void TurnOffZoomDirectly()
    {
        if (_state == State.Idle) return;

        _state = State.Idle;
        OnZoomRequested?.Invoke(false);
        Zoom?.Invoke(true, 0, Vector3.zero, _normalFieldOfView);
    }
}


abstract public class ApplyAttack : ActionStrategy
{
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

    public ApplyAttack(BaseWeapon.Name weaponName, float range, int targetLayer, 
        Action<string, int, float> OnPlayWeaponAnimation, Action<SoundType, bool> PlaySound) : base(PlaySound)
    {
        _weaponName = weaponName;
        _range = range;
        _targetLayer = targetLayer;

        this.OnPlayWeaponAnimation = OnPlayWeaponAnimation;
    }

    protected void PlayAnimation(string aniName)
    {
        OnPlayOwnerAnimation?.Invoke(_weaponName.ToString() + aniName, 0, 0);
        OnPlayWeaponAnimation?.Invoke(aniName, -1, 0);
    }

    protected void PlayAnimation(string aniName, int index)
    {
        OnPlayOwnerAnimation?.Invoke(_weaponName.ToString() + aniName + index, -1, 0);
        OnPlayWeaponAnimation?.Invoke(aniName + index, -1, 0);
    }


    // 데미지를 적용하는 함수는 여기에 만들어주기 ex) 총기형 데미지 함수, 칼 데미지 함수
    protected virtual float CalculateDamage(IHitable hitable, PenetrateData data, float decreaseRatio) { return default; }

    protected virtual float CalculateDamage(IHitable hitable) { return default; }

    protected virtual void ApplyDamage(IHitable hitable, RaycastHit hit) { }

    protected virtual void ApplyDamage(IHitable hitable, PenetrateData data, float decreaseRatio) { }

    protected virtual void SpawnEffect(string name, Vector3 position) { }
    protected virtual void SpawnEffect(string name, Vector3 position, Vector3 normal) { }
    protected virtual void SpawnEffect(string name, Vector3 position, Vector3 normal, bool isBlocked) { }
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

    /// <summary>
    /// 이동 변위 수치를 받아온다.
    /// </summary>
    protected Func<float> ReceiveMoveDisplacement;

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

    protected float _additionalWeight;

    public float DisplacementWeight { get { return ReceiveMoveDisplacement() + _additionalWeight; }}


    protected Action<Vector3> OnNoiseGenerateRequested;


    public PenetrateAttack(BaseWeapon.Name weaponName, float range, int targetLayer, int fireCountInOnce,
        float penetratePower, float displacementDecreaseRatio, Dictionary<HitArea, DistanceAreaData[]> damageDictionary,

        Action<string, int, float> OnPlayWeaponAnimation, Func<Vector3> ReturnMuzzlePos, Func<int> ReturnLeftAmmoCount,
        Action<int> DecreaseAmmoCount, Action SpawnMuzzleFlashEffect, Action SpawnEmptyCartridge, Action<Vector3> OnNoiseGenerateRequested, Action<SoundType, bool> PlaySound)

        : base(weaponName, range, targetLayer, OnPlayWeaponAnimation, PlaySound)
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
        bulletHoleEffect = ObjectPool.Spawn<BaseEffect>(name);

        bulletHoleEffect.Initialize(hitPosition, hitNormal, Quaternion.LookRotation(-hitNormal));
        bulletHoleEffect.PlayEffect();
    }

    protected override void SpawnEffect(string name, Vector3 hitPosition)
    {
        BaseEffect bulletHoleEffect;
        bulletHoleEffect = ObjectPool.Spawn<BaseEffect>(name);

        bulletHoleEffect.Initialize(hitPosition);
        bulletHoleEffect.PlayEffect();
    }

    void DrawTrajectoryLine(Vector3 hitPosition)
    {
        Vector3 muzzlePos = ReturnMuzzlePos();

        BaseEffect trajectoryLineEffect = ObjectPool.Spawn<BaseEffect>(_trajectoryLineEffect);
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
        PlaySound?.Invoke(SoundType.Attack, true);

        // 여기에 사운드 발생 기능 추가
        PlayAnimation("Fire");
        PlayEffect();
    }

    public override void UnlinkEvent(WeaponEventBlackboard blackboard)
    {
        ReturnRaycastPos -= blackboard.ReturnRaycastPos;
        ReturnRaycastDir -= blackboard.ReturnRaycastDir;
        OnPlayOwnerAnimation -= blackboard.OnPlayOwnerAnimation;
        ReceiveMoveDisplacement -= blackboard.SendMoveDisplacement;
    }

    public override void LinkEvent(WeaponEventBlackboard blackboard)
    {
        ReturnRaycastPos += blackboard.ReturnRaycastPos;
        ReturnRaycastDir += blackboard.ReturnRaycastDir;
        OnPlayOwnerAnimation += blackboard.OnPlayOwnerAnimation;

        ReceiveMoveDisplacement += blackboard.SendMoveDisplacement;
    }

    //public void OnDisplacementWeightReceived(float displacement)
    //{
    //    _displacementWeight = displacement * _displacementDecreaseRatio;
    //}

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
        float penetratePower, float displacementDecreaseRatio, Dictionary<HitArea, DistanceAreaData[]> damageDictionary,

        Action<string, int, float> OnPlayWeaponAnimation, Func<Vector3> ReturnMuzzlePos, Func<int> ReturnLeftAmmoCount,
        Action<int> DecreaseAmmoCount, Action SpawnMuzzleFlashEffect, Action SpawnEmptyCartridge, Action<Vector3> OnNoiseGenerateRequested, Action<SoundType, bool> PlaySound)

        : base(weaponName, range, targetLayer, fireCountInOnce, penetratePower, displacementDecreaseRatio, 
            damageDictionary, OnPlayWeaponAnimation, ReturnMuzzlePos, ReturnLeftAmmoCount,
            DecreaseAmmoCount, SpawnMuzzleFlashEffect, SpawnEmptyCartridge, OnNoiseGenerateRequested, PlaySound)
    {
    }

    protected Vector3 ReturnOffset(float weight)
    {
        float x = Random.Range(-weight, weight);
        float y = Random.Range(-weight, weight);

        return new Vector3(x, y, 0);
    }

    public override void Execute()
    {
        base.Execute();
        Vector3 multifliedOffset = ReturnOffset(DisplacementWeight);
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

    public SingleProjectileAttackWithWeight(BaseWeapon.Name weaponName, float range, int targetLayer, int fireCountInOnce,
        float penetratePower, float displacementDecreaseRatio, WeightApplier weightApplier, Dictionary<HitArea, DistanceAreaData[]> damageDictionary,
        

        Action<string, int, float> OnPlayWeaponAnimation, Func<Vector3> ReturnMuzzlePos, Func<int> ReturnLeftAmmoCount,
        Action<int> DecreaseAmmoCount, Action SpawnMuzzleFlashEffect, Action SpawnEmptyCartridge, Action<Vector3> OnNoiseGenerateRequested, Action<SoundType, bool> PlaySound)

        : base(weaponName, range, targetLayer, fireCountInOnce, penetratePower, displacementDecreaseRatio, 
            damageDictionary, OnPlayWeaponAnimation, ReturnMuzzlePos, ReturnLeftAmmoCount,
            DecreaseAmmoCount, SpawnMuzzleFlashEffect, SpawnEmptyCartridge, OnNoiseGenerateRequested, PlaySound)
    {
        _weightApplier = weightApplier;
    }

    public override void Execute()
    {
        _additionalWeight = _weightApplier.StoredWeight;
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

    public ScatterProjectileAttack(BaseWeapon.Name weaponName, float range, int targetLayer, int fireCountInOnce,
        float penetratePower, float displacementDecreaseRatio, int pelletCount, float spreadOffset, //int nextFireCount,
        Dictionary<HitArea, DistanceAreaData[]> damageDictionary,

        Action<string, int, float> OnPlayWeaponAnimation, Func<Vector3> ReturnMuzzlePos, Func<int> ReturnLeftAmmoCount,
        Action<int> DecreaseAmmoCount, Action SpawnMuzzleFlashEffect, Action SpawnEmptyCartridge, Action<Vector3> OnNoiseGenerateRequested, Action<SoundType, bool> PlaySound)

        : base(weaponName, range, targetLayer, fireCountInOnce, penetratePower, displacementDecreaseRatio, 
            damageDictionary, OnPlayWeaponAnimation, ReturnMuzzlePos, ReturnLeftAmmoCount,
            DecreaseAmmoCount, SpawnMuzzleFlashEffect, SpawnEmptyCartridge, OnNoiseGenerateRequested, PlaySound)
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
            float z = Random.Range(-_spreadOffset - weight, _spreadOffset + weight);

            offsetDistance.Add(new Vector3(x, y, z));
        }


        return offsetDistance;
    }

    public override void Execute()
    {
        int leftAmmoCount = ReturnLeftAmmoCount();
        if (_storedFireCount > leftAmmoCount) _fireCountInOnce = leftAmmoCount; //  _fireCountInOnce 재지정
        else _fireCountInOnce = _storedFireCount;

        base.Execute();

        List<Vector3> offsetDistances = ReturnOffsetDistance(DisplacementWeight, _pelletCount * _fireCountInOnce);
        for (int i = 0; i < offsetDistances.Count; i++)
        {
            Shoot(offsetDistances[i], _frontPosition);
        }
    }
}

public class ScatterProjectileAttackWithWeight : ScatterProjectileAttack
{
    WeightApplier _weightApplier;

    public ScatterProjectileAttackWithWeight(BaseWeapon.Name weaponName, float range, int targetLayer, int fireCountInOnce,
        float penetratePower, float displacementDecreaseRatio, int pelletCount, float spreadOffset, WeightApplier weightApplier,
        Dictionary<HitArea, DistanceAreaData[]> damageDictionary,

        Action<string, int, float> OnPlayWeaponAnimation, Func<Vector3> ReturnMuzzlePos, Func<int> ReturnLeftAmmoCount,
        Action<int> DecreaseAmmoCount, Action SpawnMuzzleFlashEffect, Action SpawnEmptyCartridge, Action<Vector3> OnNoiseGenerateRequested, Action<SoundType, bool> PlaySound)

        : base(weaponName, range, targetLayer, fireCountInOnce, penetratePower, displacementDecreaseRatio, pelletCount, 
            spreadOffset, damageDictionary, OnPlayWeaponAnimation, ReturnMuzzlePos, ReturnLeftAmmoCount,
            DecreaseAmmoCount, SpawnMuzzleFlashEffect, SpawnEmptyCartridge, OnNoiseGenerateRequested, PlaySound)
    {
        _weightApplier = weightApplier;
    }

    public override void Execute()
    {
        _additionalWeight = _weightApplier.StoredWeight;
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
    float _frontDistance;

    public ExplosionScatterProjectileAttack(BaseWeapon.Name weaponName, float range, int targetLayer, int fireCountInOnce,
        float penetratePower, float displacementDecreaseRatio, int pelletCount, float spreadOffset, float frontDistance,
        string explosionEffectName, Dictionary<HitArea, DistanceAreaData[]> damageDictionary,

        Action<string, int, float> OnPlayWeaponAnimation, Func<Vector3> ReturnMuzzlePos, Func<int> ReturnLeftAmmoCount,
        Action<int> DecreaseAmmoCount, Action SpawnMuzzleFlashEffect, Action SpawnEmptyCartridge, Action<Vector3> OnNoiseGenerateRequested, Action<SoundType, bool> PlaySound)

        : base(weaponName, range, targetLayer, fireCountInOnce, penetratePower, displacementDecreaseRatio, pelletCount, spreadOffset,
            damageDictionary, OnPlayWeaponAnimation, ReturnMuzzlePos, ReturnLeftAmmoCount,
            DecreaseAmmoCount, SpawnMuzzleFlashEffect, SpawnEmptyCartridge, OnNoiseGenerateRequested, PlaySound)
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

        Dictionary<HitArea, DistanceAreaData[]> damageDictionary,
        Dictionary<HitArea, DistanceAreaData[]> scatterDamageDictionary, float findRange,


        Action<string, int, float> OnPlayWeaponAnimation, Func<Vector3> ReturnMuzzlePos, Func<int> ReturnLeftAmmoCount,
        Action<int> DecreaseAmmoCount, Action SpawnMuzzleFlashEffect, Action SpawnEmptyCartridge, Action<Vector3> OnNoiseGenerateRequested, Action<SoundType, bool> PlaySound
        ) : base(PlaySound)
    {
        _targetLayer = targetLayer;
        _findRange = findRange;

        singleProjectileAttack = new SingleProjectileAttack(weaponName, range, targetLayer, singleBulletCountsInOneShoot, singlePenetratePower, singleDisplacementDecreaseRatio,
            damageDictionary, OnPlayWeaponAnimation, ReturnMuzzlePos, ReturnLeftAmmoCount, DecreaseAmmoCount, 
            SpawnMuzzleFlashEffect, SpawnEmptyCartridge, OnNoiseGenerateRequested, PlaySound);

        scatterProjectileGunAttack = new ExplosionScatterProjectileAttack(weaponName, range, targetLayer, scatterBulletCountsInOneShoot, scatterPenetratePower, scatterDisplacementDecreaseRatio,
            pelletCount, spreadOffset, frontDistance, explosionEffectName, scatterDamageDictionary, OnPlayWeaponAnimation, ReturnMuzzlePos, ReturnLeftAmmoCount, DecreaseAmmoCount,
            SpawnMuzzleFlashEffect, SpawnEmptyCartridge, OnNoiseGenerateRequested, PlaySound);
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
        
        Action<string, int, float> OnPlayWeaponAnimation, Action<SoundType, bool> PlaySound) 
        : base(weaponName, range, targetLayer, OnPlayWeaponAnimation, PlaySound)
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

        PlaySound?.Invoke(SoundType.Attack, true);

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
        BaseEffect effect = ObjectPool.Spawn<BaseEffect>(name);

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

        Action<string, int, float> OnPlayWeaponAnimation, Action<SoundType, bool> PlaySound)
        : base(weaponName, range, targetLayer, delayForNextStab, directionData, OnPlayWeaponAnimation, PlaySound)
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

        Action<string, int, float> OnPlayWeaponAnimation, Action<SoundType, bool> PlaySound)
        : base(weaponName, range, targetLayer, delayForNextStab, directionData, OnPlayWeaponAnimation, PlaySound)
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