using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

abstract public class Gun : BaseWeapon, IInteractable
{
    [SerializeField]
    protected ParticleSystem _muzzleFlash;

    [SerializeField]
    protected ParticleSystem _emptyCartridgeSpawner;

    protected float _penetratePower = 15;
    protected float _trajectoryLineOffset = 1.3f;

    [SerializeField]
    protected Transform _muzzle;

    protected string _trajectoryLineEffect = "TrajectoryLine";

    [SerializeField]
    protected int _maxAmmoCountInMagazine = 30;

    [SerializeField]
    protected int _maxAmmoCountsInPossession = 60;

    protected int _ammoCountsInMagazine;

    protected int _ammoCountsInPossession;

    [SerializeField]
    protected float _reloadFinishTime;

    [SerializeField]
    protected float _reloadExitTime;

    [SerializeField]
    Transform _objectMesh;

    BoxCollider _gunCollider;
    Rigidbody _gunRigidbody;
    bool _nowAttachToGround;

    public Action<bool, string, Vector3> OnViewEventRequest;

    public override bool CanDrop() { return true; }

    public override bool NowNeedToRefillAmmo() { return _ammoCountsInMagazine == 0 && _ammoCountsInPossession == 0; }

    public override bool NeedToReload() { return _ammoCountsInMagazine == 0 && _ammoCountsInPossession > 0; }

    public override bool CanAttack() { return _ammoCountsInMagazine > 0; }

    public override void RefillAmmo() 
    {
        //_reloadStrategy.OnResetReload(); // 리로드 중이면 취소하고 진행

        _ammoCountsInMagazine = _maxAmmoCountInMagazine;
        _ammoCountsInPossession = _maxAmmoCountsInPossession;
        OnRoundChangeRequested?.Invoke(true, _ammoCountsInMagazine, _ammoCountsInPossession);
    }

    /// AutoReload 이벤트
    ////////////////////////////////////////////////////////////////////////////////////

    public override bool CanAutoReload()
    {
        return _ammoCountsInMagazine == 0 && _ammoCountsInPossession > 0;
    }

    /// Reload 이벤트
    ////////////////////////////////////////////////////////////////////////////////////

    public override bool CanReload()
    {
        // 탄창이 꽉 차거나 소유 중인 총알이 0보다 같거나 작은 경우
        if (_ammoCountsInPossession <= 0 || _maxAmmoCountInMagazine == _ammoCountsInMagazine) return false;
        else return true;
    }

    public override void OnReload()
    {
        _mainResultStrategy.OnReload(); // 에임 해제
        _subResultStrategy.OnReload();
        _reloadStrategy.Reload(_ammoCountsInMagazine, _ammoCountsInPossession);
    }

    // 장전이 끝나면 여기 이벤트 호출됨
    protected void OnReloadRequested(int ammoInMagazine, int ammoInPossession)
    {
        _ammoCountsInMagazine = ammoInMagazine;
        _ammoCountsInPossession = ammoInPossession;
        OnRoundChangeRequested?.Invoke(true, _ammoCountsInMagazine, _ammoCountsInPossession);
    }

    ////////////////////////////////////////////////////////////////////////////////////

    public override void ThrowGun(float force)
    {
        PositionWeaponMesh(true);
        _gunRigidbody.AddForce(_camTransform.forward * force, ForceMode.Impulse);
    }

    public override void PositionWeaponMesh(bool nowDrop)
    {
        if(nowDrop)
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


    public override void Initialize(GameObject player, GameObject armMesh, Transform cam, Animator ownerAnimator)
    {
        base.Initialize(player, armMesh, cam, ownerAnimator);

        _gunCollider = GetComponent<BoxCollider>();
        _gunRigidbody = GetComponent<Rigidbody>();

        _gunCollider.enabled = false;
        _gunRigidbody.isKinematic = true;

        _ammoCountsInMagazine = _maxAmmoCountInMagazine;
        _ammoCountsInPossession = _maxAmmoCountsInPossession;

        GameObject viewer = GameObject.FindWithTag("WeaponInfoViewer");
        if (viewer == null) return;

        WeaponInfoViwer weaponInfoViwer = viewer.GetComponent<WeaponInfoViwer>();
        if (weaponInfoViwer == null) return; 

        OnViewEventRequest = weaponInfoViwer.OnViewEventReceived; // 드랍 시 해제 필요
    }

    protected override void OnMainActionEventCallRequsted()
    {
        _mainResultStrategy.CheckBulletLeftCount(_ammoCountsInMagazine); // 여기서 남은 총알을 체크함

        base.OnMainActionEventCallRequsted();
        _ammoCountsInMagazine = _mainResultStrategy.DecreaseBullet(_ammoCountsInMagazine); // 발사 시 총알 감소 적용
        OnRoundChangeRequested?.Invoke(true, _ammoCountsInMagazine, _ammoCountsInPossession);

        _subResultStrategy.TurnOffZoomWhenOtherExecute(); // 발사 후 줌 해제 적용
    }

    protected override void OnSubActionEventCallRequsted()
    {
        _subResultStrategy.CheckBulletLeftCount(_ammoCountsInMagazine); // 여기서 남은 총알을 체크함

        base.OnSubActionEventCallRequsted();
        _ammoCountsInMagazine = _subResultStrategy.DecreaseBullet(_ammoCountsInMagazine); // 발사 시 총알 감소 적용
        OnRoundChangeRequested?.Invoke(true, _ammoCountsInMagazine, _ammoCountsInPossession);

        _mainResultStrategy.TurnOffZoomWhenOtherExecute(); // 발사 후 줌 해제 적용
    }

    public override void OnEquip()
    {
        base.OnEquip();
        OnRoundChangeRequested?.Invoke(true, _ammoCountsInMagazine, _ammoCountsInPossession);
    }

    protected void OnZoomEventCall(bool nowZoom)
    {
        if (nowZoom) OnZoomIn();
        else OnZoomOut();
    }

    protected virtual void OnZoomIn() { }

    protected virtual void OnZoomOut() { }

    public void OnInteract()
    {

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