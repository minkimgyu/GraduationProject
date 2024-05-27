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
    /// Action�� ȣ���� �� �ִ��� Ȯ���ϴ� �Լ�
    /// </summary>
    public virtual bool CanExecute() { return true; }

    /// <summary>
    /// Action�� ȣ���� �� ����ϴ� �Լ�
    /// </summary>
    public virtual void Execute() { }

    /// <summary>
    /// Aim�� ������ �� ȣ��Ǵ� �Լ�
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
    /// ���� �Ǹ� Action�� �ٲ��� --> ���� �ӵ� �� ���� ��ġ ��������� �ϱ� ������
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
                // ���⿡�� ���� ����
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
    /// ī�޶� ��ġ�� ��ȯ���ش�.
    /// </summary>
    protected Func<Vector3> ReturnRaycastPos;

    /// <summary>
    /// ī�޶� ������ ��ȯ���ش�.
    /// </summary>
    protected Func<Vector3> ReturnRaycastDir;

    protected float _range;
    protected int _targetLayer;
    protected BaseDamageConverter _damageConverter;

    /// <summary>
    /// ������ �ִϸ��̼��� �����ų �� ȣ��
    /// </summary>
    Action<string, int, float> OnPlayWeaponAnimation;

    /// <summary>
    /// ���⸦ ������ ����� �ִϸ��̼��� �����ų �� ȣ��
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


    // �������� �����ϴ� �Լ��� ���⿡ ������ֱ� ex) �ѱ��� ������ �Լ�, Į ������ �Լ�
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
    /// ������ ��ġ�� ��ȯ�Ѵ�.
    /// </summary>
    Func<Vector3> ReturnMuzzlePos;

    /// <summary>
    /// ���� �Ѿ� ���� ��ȯ�Ѵ�.
    /// </summary>
    protected Func<int> ReturnLeftAmmoCount;

    /// <summary>
    /// fireCountInOnce ��ŭ �Ѿ��� ���ҽ�Ų��.
    /// </summary>
    Action<int> DecreaseAmmoCount;

    /// <summary>
    /// �ѱ� ȭ���� ������Ų��.
    /// </summary>
    Action SpawnMuzzleFlashEffect;

    /// <summary>
    /// ź�Ǹ� ������Ų��.
    /// </summary>
    Action SpawnEmptyCartridge;

    /// <summary>
    /// �̵� ���� ��ġ�� �޾ƿ´�.
    /// </summary>
    protected Func<float> ReceiveMoveDisplacement;

    protected float _penetratePower;

    protected float _minDecreaseRatio = 0.05f;
    protected float _trajectoryLineOffset = 1.3f;

    protected string _trajectoryLineEffect;

    /// <summary>
    /// �׼� 1���� �Ҹ�Ǵ� �Ѿ��� ����
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

        Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance)); // ���� ��ü���� �Ÿ� ������� �������ش�.

        return hits;
    }

    protected List<PenetrateData> ReturnPenetrateData(Vector3 directionOffset = default(Vector3), Vector3 positionOffset = default(Vector3))
    {
        Vector3 camPos = Vector3.zero;
        Vector3 camFowardDir = Vector3.zero;

        camPos = ReturnRaycastPos();
        camFowardDir = ReturnRaycastDir();

        RaycastHit[] entryHits = DetectCollider(camPos, camFowardDir + directionOffset, _range, _targetLayer);

        if (entryHits.Length == 0) return null; // ���� �ƹ��� ���� �ʾҴٸ� ����

        Vector3 endPoint = camPos + (camFowardDir + directionOffset) * _range; // ����ĳ��Ʈ�� �ݴ� ���� ������ ��ġ

        RaycastHit[] exitHits = DetectCollider(endPoint, -(camFowardDir + directionOffset), _range, _targetLayer);

        List<PenetrateData> penetrateDatas = new List<PenetrateData>();

        for (int i = 0; i < entryHits.Length; i++) // ���� ������ 
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

        if (decreaseRatio > _minDecreaseRatio) // decreaseRatio�� 5% �̻��� ��츸 �ش�
        {
            damage -= Mathf.Round(damage * decreaseRatio);
        }

        return damage;
    }

    protected override void ApplyDamage(IHitable hit, PenetrateData data, float decreaseRatio)
    {
        float damage = CalculateDamage(hit, data, decreaseRatio);
        hit.OnHit(damage, data.EntryPoint, data.EntryNormal); // ������ ����
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

        Debug.DrawRay(camPos, direction * diatance, Color.green, 10); // ����� ���̸� ī�޶� �ٶ󺸰� �ִ� �������� �߻��Ѵ�.
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
            // �� ���, ����, ���� ����Ʈ ��ο� ���� �̹��� �߰�
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
            // ����� ������Ʈ�� ������
            GameObject target = penetrateDatas[i].Target.ReturnAttachedObject();

            // IHitable ������
            IHitable hitable;
            target.TryGetComponent(out hitable);

            //IEffectable ������
            IEffectable effectable;
            target.TryGetComponent(out effectable);
            if (effectable == null) continue;

            if(hitable != null)
            {
                // �̹� �ش� ������Ʈ�� �������� ����Ǿ����� Ȯ��
                bool isAlreadyDamaged = CheckIsAlreadyDamaged(hitable, alreadyDamagedObjects);
                if (isAlreadyDamaged) continue;
            }

            // ������Ʈ�� �������� ������
            float finalDurability = penetrateDatas[i].ReturnFinalDurability();

            bool IsLastContact = false;
            if (i == penetrateDatas.Count - 1) IsLastContact = true; // ������ �浹 ��
           
            // ���� ��ġ�� 0�̸� break
            tmpPenetratePower = CalculatePenetratePower(tmpPenetratePower, finalDurability, effectable, penetrateDatas[i], IsLastContact);
            if (tmpPenetratePower == 0) break;

            if (hitable != null)
            {
                // ������ ���� ����
                float decreasePowerRatio = (_penetratePower - tmpPenetratePower) / _penetratePower;
                if (hitable != null) ApplyDamage(hitable, penetrateDatas[i], decreasePowerRatio);
            }

            if (penetrateDatas.Count <= i + 1) continue; // �ڿ� ���� ������ ���ٸ� �������� ����

            // �������� ����� ������Ʈ ������ �Ÿ��� ���� ���� ��ġ�� ���� ����
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
        DecreaseAmmoCount?.Invoke(_fireCountInOnce); // �Ѿ� ����

        Vector3 muzzlePos = ReturnMuzzlePos();
        // _muzzle ���
        OnNoiseGenerateRequested?.Invoke(muzzlePos);
        PlaySound?.Invoke(SoundType.Attack, true);

        // ���⿡ ���� �߻� ��� �߰�
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

// WithWeight�� �ӵ� ������ ���̸� �޾Ƽ� ��������ִ� �������̽�
//public interface IDisplacement
//{
//    //float DisplacementWeight { get; set; }

//    //float DisplacementDecreaseRatio { get; set; }

//    void OnDisplacementWeightReceived(float displacement);
//}

// ���簡���� �ѿ��� ź������ ������� --> �ݵ����� ó���ϱ� ������
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

// ���� ������ �ѿ��� ź ���� �߰�

// WithWeight�� ź ������ �ǹ���
public class SingleProjectileAttackWithWeight : SingleProjectileAttack // ����ġ�� ����Ǵ� ����
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

public class ScatterProjectileAttack : PenetrateAttack // ��ź�� ����ġ�� ������� ����
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
        _storedFireCount = _fireCountInOnce; // _storedFireCount�� �����صд�.
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
        if (_storedFireCount > leftAmmoCount) _fireCountInOnce = leftAmmoCount; //  _fireCountInOnce ������
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

public class ExplosionScatterProjectileAttack : ScatterProjectileAttack // ��ź�� ����ġ�� ������� ����
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

        // �ѱ� �տ� ����Ʈ ����
        SpawnEffect(_explosionEffectName, camPos + (camFowardDir * _frontPosition.z));
        base.Execute();
    }
}

public class SingleAndExplosionScatterAttackCombination : ActionStrategy // Attack�� �ƴ϶� �ٸ� ������� ����ؾ��� ��
{
    SingleProjectileAttack singleProjectileAttack;
    ScatterProjectileAttack scatterProjectileGunAttack;
    // �� �� ���� �����ڿ��� �ʱ�ȭ
    // ����, Ÿ�Կ� �°� ����

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

// Į ������ ���⼭ ����
abstract public class BaseKnifeAttack : ApplyAttack
{
    float _delayForNextStab;
    StopwatchTimer _delayTimer; // ���� �� �۵�

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