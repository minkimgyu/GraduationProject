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
        //_reloadStrategy.OnResetReload(); // ���ε� ���̸� ����ϰ� ����

        _ammoCountsInMagazine = _maxAmmoCountInMagazine;
        _ammoCountsInPossession = _maxAmmoCountsInPossession;
        OnRoundChangeRequested?.Invoke(true, _ammoCountsInMagazine, _ammoCountsInPossession);
    }

    /// AutoReload �̺�Ʈ
    ////////////////////////////////////////////////////////////////////////////////////

    public override bool CanAutoReload()
    {
        return _ammoCountsInMagazine == 0 && _ammoCountsInPossession > 0;
    }

    /// Reload �̺�Ʈ
    ////////////////////////////////////////////////////////////////////////////////////

    public override bool CanReload()
    {
        // źâ�� �� ���ų� ���� ���� �Ѿ��� 0���� ���ų� ���� ���
        if (_ammoCountsInPossession <= 0 || _maxAmmoCountInMagazine == _ammoCountsInMagazine) return false;
        else return true;
    }

    public override void OnReload()
    {
        _mainResultStrategy.OnReload(); // ���� ����
        _subResultStrategy.OnReload();
        _reloadStrategy.Reload(_ammoCountsInMagazine, _ammoCountsInPossession);
    }

    // ������ ������ ���� �̺�Ʈ ȣ���
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

        OnViewEventRequest = weaponInfoViwer.OnViewEventReceived; // ��� �� ���� �ʿ�
    }

    protected override void OnMainActionEventCallRequsted()
    {
        _mainResultStrategy.CheckBulletLeftCount(_ammoCountsInMagazine); // ���⼭ ���� �Ѿ��� üũ��

        base.OnMainActionEventCallRequsted();
        _ammoCountsInMagazine = _mainResultStrategy.DecreaseBullet(_ammoCountsInMagazine); // �߻� �� �Ѿ� ���� ����
        OnRoundChangeRequested?.Invoke(true, _ammoCountsInMagazine, _ammoCountsInPossession);

        _subResultStrategy.TurnOffZoomWhenOtherExecute(); // �߻� �� �� ���� ����
    }

    protected override void OnSubActionEventCallRequsted()
    {
        _subResultStrategy.CheckBulletLeftCount(_ammoCountsInMagazine); // ���⼭ ���� �Ѿ��� üũ��

        base.OnSubActionEventCallRequsted();
        _ammoCountsInMagazine = _subResultStrategy.DecreaseBullet(_ammoCountsInMagazine); // �߻� �� �Ѿ� ���� ����
        OnRoundChangeRequested?.Invoke(true, _ammoCountsInMagazine, _ammoCountsInPossession);

        _mainResultStrategy.TurnOffZoomWhenOtherExecute(); // �߻� �� �� ���� ����
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
        _nowAttachToGround = true; // ���� �΋H���� �׶����� Interaction ����
    }

    public bool IsInteractable() { return _nowAttachToGround; }

    public T ReturnComponent<T>() { return GetComponent<T>(); }
}