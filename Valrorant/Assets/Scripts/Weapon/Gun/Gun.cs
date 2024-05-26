using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Agent.Controller;

abstract public class Gun : BaseWeapon, IInteractable
{
    // 드랍 시 아이템을 버리는 방향
    protected Func<Vector3> ReturnRaycastDir;

    [SerializeField]
    protected ParticleSystem _muzzleFlash;

    [SerializeField]
    protected ParticleSystem _emptyCartridgeSpawner;

    //protected float _penetratePower = 15;
    protected float _trajectoryLineOffset = 1.3f;

    [SerializeField] protected Transform _muzzle;


    protected string _trajectoryLineEffect = "TrajectoryLine";

    protected int _maxAmmoCountInMagazine;

    protected int _maxAmmoCountsInPossession;

    [SerializeField] protected int _ammoCountsInMagazine;

    [SerializeField] protected int _ammoCountsInPossession;

    [SerializeField] Transform _objectMesh;

    BoxCollider _gunCollider;
    Rigidbody _gunRigidbody;
    bool _nowAttachToGround;

    public Action<bool, string, Vector3> OnViewEventRequest;

    protected Action<Vector3> OnGenerateNoiseRequest;

    protected void SpawnEmptyCartridge() => _emptyCartridgeSpawner.Play();
    protected void SpawnMuzzleFlashEffect() => _muzzleFlash.Play();

    public override bool IsAmmoEmpty()
    {
        return _ammoCountsInMagazine == 0 && _ammoCountsInPossession == 0;
    }

    public void DecreaseAmmoCount(int _fireCountInOnce)
    {
        _ammoCountsInMagazine -= _fireCountInOnce;
        if (_ammoCountsInMagazine < 0) _ammoCountsInMagazine = 0;
    }

    public int ReturnLeftAmmoCount() { return _ammoCountsInMagazine; }

    protected Vector3 ReturnMuzzlePos() { return _muzzle.position; }


    #region Reload

    public override bool CanAutoReload() { return _reloadStrategy.CanAutoReload(_ammoCountsInMagazine, _ammoCountsInPossession); }

    public override bool CanReload() { return _reloadStrategy.CanReload(_ammoCountsInMagazine, _ammoCountsInPossession, _maxAmmoCountInMagazine); }

    public override void OnReload(bool isTPS)
    {
        for (int i = 0; i < _actionStrategies.Count; i++) _actionStrategies[(EventType)i].TurnOffZoomDirectly();
        _reloadStrategy.Execute(isTPS, _ammoCountsInMagazine, _ammoCountsInPossession);
    }

    // 장전이 끝나면 여기 이벤트 호출됨
    protected void OnReloadRequested(int ammoInMagazine, int ammoInPossession)
    {
        _ammoCountsInMagazine = ammoInMagazine;
        _ammoCountsInPossession = ammoInPossession;
        _weaponEventBlackboard.OnShowRounds?.Invoke(true, _ammoCountsInMagazine, _ammoCountsInPossession);
    }

    #endregion


    #region Drop

    public override bool CanDrop() { return true; }

    public override void ThrowWeapon(float force)
    {
        PositionWeapon(true);
        _gunRigidbody.AddForce(ReturnRaycastDir() * force, ForceMode.Impulse);
    }

    public override void PositionWeapon(bool nowDrop)
    {
        if (nowDrop)
        {
            _gunCollider.enabled = true;
            _gunRigidbody.isKinematic = false;
        }
        else
        {
            _nowAttachToGround = false;
            _gunCollider.enabled = false;
            _gunRigidbody.isKinematic = true;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            gameObject.SetActive(false);
        }
    }

    #endregion

    #region Equip

    public override void OnEquip()
    {
        base.OnEquip();
        _weaponEventBlackboard.OnShowRounds?.Invoke(true, _ammoCountsInMagazine, _ammoCountsInPossession);
    }

    #endregion;

    public override void OnRooting(WeaponEventBlackboard blackboard)  // 
    {
        base.OnRooting(blackboard);
        ReturnRaycastDir += blackboard.ReturnRaycastDir;
    }

    public override void OnDrop(WeaponEventBlackboard blackboard)
    {
        base.OnDrop(blackboard);
        ReturnRaycastDir -= blackboard.ReturnRaycastDir;
    }

    public override void SetDefaultValue()
    {
        base.SetDefaultValue();

        _gunCollider = GetComponent<BoxCollider>();
        _gunRigidbody = GetComponent<Rigidbody>();

        _gunCollider.enabled = false;
        _gunRigidbody.isKinematic = true;

        WeaponInfoViwer weaponInfoViwer = FindObjectOfType<WeaponInfoViwer>();
        if (weaponInfoViwer == null) return;
        OnViewEventRequest = weaponInfoViwer.OnViewEventReceived; // 드랍 시 해제 필요

        NoiseGenerator noiseGenerator = FindObjectOfType<NoiseGenerator>();
        OnGenerateNoiseRequest = noiseGenerator.GenerateNoise;
    }

    public override void RefillAmmo() 
    {
        _ammoCountsInMagazine = _maxAmmoCountInMagazine;
        _ammoCountsInPossession = _maxAmmoCountsInPossession;

        _weaponEventBlackboard.OnShowRounds?.Invoke(true, _ammoCountsInMagazine, _ammoCountsInPossession);
    }

    protected override void OnAction(EventType type)
    {
        base.OnAction(type);
        _weaponEventBlackboard.OnShowRounds?.Invoke(true, _ammoCountsInMagazine, _ammoCountsInPossession);
    }

    public void OnSightEnter()
    {
        OnViewEventRequest?.Invoke(true, _weaponName.ToString(), _objectMesh.position);
    }

    public void OnSightExit()
    {
        OnViewEventRequest?.Invoke(false, _weaponName.ToString(), _objectMesh.position);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        _nowAttachToGround = true; // 어디든 부딫히면 그때부터 Interaction 적용
    }

    public bool IsInteractable() { return _nowAttachToGround; }

    public T ReturnComponent<T>() { return GetComponent<T>(); }
}