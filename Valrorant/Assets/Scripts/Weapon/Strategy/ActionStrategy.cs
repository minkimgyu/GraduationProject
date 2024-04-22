using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;
using System;
using Random = UnityEngine.Random;

abstract public class ActionStrategy : BaseStrategy
{

    /// <summary>
    /// Action�� ȣ���ϱ� �� ���� �Ѿ� ������ �ʱ�ȭ �ϴ� �Լ�
    /// </summary>
    //public virtual void ResetLeftBulletCount(int leftBulletCount) { }

    /// <summary>
    /// Action�� ȣ���� �� �Ѿ��� ���ҽ�Ű�� �Լ�
    /// </summary>
    //public virtual int DecreaseBullet(int ammoCountsInMagazine) { return 0; }

    /// <summary>
    /// Action�� ȣ���� �� �ִ��� Ȯ���ϴ� �Լ�
    /// </summary>
    public virtual bool CanExecute() { return true; }

    /// <summary>
    /// Action�� ȣ���� �� ����ϴ� �Լ�
    /// </summary>
    public virtual void Execute() { }

    // �̺�Ʈ�� �޴� �Լ��� abstract �̿��ؼ� �Լ��� ������ � ����� �ϴ��� Ȯ���� �����ֱ�
    //public abstract void LinkEvent(GameObject player);

    //public abstract void UnlinkEvent(GameObject player);

    /// <summary>
    /// Reload�� ȣ��� ��� �۵��ϴ� �Լ�
    /// </summary>
    //public virtual void OnReloadRequested() { }

    /// <summary>
    /// Unequip�� ȣ��� ��� �۵��ϴ� �Լ�
    /// </summary>
    //public virtual void OnUnequipRequested() { }


    // �� �� �Լ��� �����ϰ� TurnOffAim �̰ɷ� ��ġ��

    /// <summary>
    /// Aim�� ������ �� ȣ��Ǵ� �Լ�
    /// </summary>
    public virtual void TurnOffZoomDirectly() { }

    //public abstract void Update();



    // ���� �Լ����� Ȯ���ϰ� ���ִ��� �����ϴ��� �غ���
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
    /// ���� �Ǹ� Action�� �ٲ��� --> ���� �ӵ� �� ���� ��ġ ��������� �ϱ� ������
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


    //    OnZoomEventCall?.Invoke(_nowTurnOn); // �� �̺�Ʈ ȣ��
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
    //scope�� Find�� ã�Ƽ� ��� --> ĵ���� �ȿ� ����

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
    //    //ActivateMesh(true); // �޽� Ȱ��ȭ
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
    //        // ���� ������ ��
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
                // ���⿡�� ���� ����
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
        //    base.Execute(); // ��ַ� ��
        //}
        //else if (_nowTurnOn == false)
        //{
        //    _disableMeshTimer.Start(_disableMeshDelay);
        //    base.Execute(); // ������ ��
        //}
    }
}


abstract public class ApplyAttack : ActionStrategy
{
    /// <summary>
    /// �̺�Ʈ�� �ٲ��ֱ�
    /// </summary>
    //protected Transform _camTransform;

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
    // �� 3���� LinkEvent�� �����ߴٰ� ���� ������� ����ؾ��Ѵ�.

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


    // �������� �����ϴ� �Լ��� ���⿡ ������ֱ� ex) �ѱ��� ������ �Լ�, Į ������ �Լ�
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

    //protected Transform _muzzle;

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

    public float DisplacementWeight { get; set; }

    //ParticleSystem _muzzleFlash;
    //ParticleSystem _emptyCartridgeSpawner;
    //bool _canSpawnMuzzleFlash;

    //int _leftBulletCount; // �߻� �� ���� �Ѿ� üũ

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

    // trajectoryLineEffect --> �̰Ŵ� �ϴ� �����ؼ� ���ٰ� ���߿� �߰��ϴ� �ɷ� ����
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
    /// �� �κ��� �ѱ� ��ũ��Ʈ�� �Ѱ��ֱ�
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
        DecreaseAmmoCount?.Invoke(_fireCountInOnce); // �Ѿ� ����

        Vector3 muzzlePos = ReturnMuzzlePos();
        // _muzzle ���
        OnNoiseGenerateRequested?.Invoke(muzzlePos);

        // ���⿡ ���� �߻� ��� �߰�
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

// ���� ������ �ѿ��� ź ���� �߰�

// WithWeight�� ź ������ �ǹ���
public class SingleProjectileAttackWithWeight : SingleProjectileAttack // ����ġ�� ����Ǵ� ����
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
        _displacementWeight += _weightApplier.StoredWeight; // ����ġ �߰� ����
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
    //    _nextFireCount = _fireCountInOnce; // �ʱ� �߻� ī��Ʈ�� _fireCountsInOneShoot�� �������ش�.

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
        _storedFireCount = _fireCountInOnce; // _storedFireCount�� �����صд�.
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
        if (_storedFireCount > leftAmmoCount) _fireCountInOnce = leftAmmoCount; //  _fireCountInOnce ������

        base.Execute();

        List<Vector3> offsetDistances = ReturnOffsetDistance(_displacementWeight, _pelletCount);
        for (int i = 0; i < offsetDistances.Count; i++)
        {
            Shoot(offsetDistances[i], _frontPosition);
        }
    }

    //public override int DecreaseBullet(int ammoCountsInMagazine)
    //{
    //    // ���� �߻��� ī��Ʈ���� ���� ī��Ʈ�� ���ٸ� ���� ī��Ʈ�� �־��ش�.
    //    if (ammoCountsInMagazine < _storedFireCount) _storedFireCount = ammoCountsInMagazine;
    //    else _storedFireCount = _fireCountInOnce;
    //    // �ƴϸ� �״�� ����

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

public class ExplosionScatterProjectileAttack : ScatterProjectileAttack // ��ź�� ����ġ�� ������� ����
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

// Į ������ ���⼭ ����
abstract public class BaseKnifeAttack : ApplyAttack
{
    float _delayForNextStab;
    StopwatchTimer _delayTimer; // ���� �� �۵�

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