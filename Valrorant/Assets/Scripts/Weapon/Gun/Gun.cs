using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

abstract public class Gun : BaseWeapon //, IInteractable
{
    [SerializeField]
    protected ParticleSystem _muzzleFlash;

    [SerializeField]
    protected ParticleSystem _emptyCartridgeSpawner;

    protected float _penetratePower = 15;
    protected float _trajectoryLineOffset = 1.3f;

    [SerializeField]
    protected Transform _muzzle;

    [SerializeField]
    protected string _trajectoryLineEffect;

    [SerializeField]
    protected int _maxAmmoCountInMagazine = 30;

    [SerializeField]
    protected int _ammoCountsInMagazine;

    [SerializeField]
    protected int _ammoCountsInPossession;

    [SerializeField]
    protected float _reloadFinishTime;

    [SerializeField]
    protected float _reloadExitTime;

    /// AutoReload 이벤트
    ////////////////////////////////////////////////////////////////////////////////////

    public override bool CanAutoReload()
    {
        return _ammoCountsInMagazine == 0;
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
        _reloadStrategy.Reload(_ammoCountsInMagazine, _ammoCountsInPossession, _maxAmmoCountInMagazine);
    }

    // 장전이 끝나면 여기 이벤트 호출됨
    protected void OnReloadRequested(int ammoInMagazine, int ammoInPossession)
    {
        _ammoCountsInMagazine = ammoInMagazine;
        _ammoCountsInPossession = ammoInPossession;
        OnRoundChangeRequested?.Invoke(true, _ammoCountsInMagazine, _ammoCountsInPossession);
    }

    ////////////////////////////////////////////////////////////////////////////////////

    public override void Initialize(GameObject player, Transform cam, Animator ownerAnimator)
    {
        base.Initialize(player, cam, ownerAnimator);
        _ammoCountsInMagazine = _maxAmmoCountInMagazine;
    }

    protected override void OnMainActionEventCallRequsted()
    {
        _mainResultStrategy.CheckBulletLeftCount(_ammoCountsInMagazine); // 여기서 남은 총알을 체크함

        base.OnMainActionEventCallRequsted();
        _ammoCountsInMagazine = _mainResultStrategy.DecreaseBullet(_ammoCountsInMagazine); // 발사 시 총알 감소 적용
        OnRoundChangeRequested?.Invoke(true, _ammoCountsInMagazine, _ammoCountsInPossession);
    }

    protected override void OnSubActionEventCallRequsted()
    {
        _subResultStrategy.CheckBulletLeftCount(_ammoCountsInMagazine); // 여기서 남은 총알을 체크함

        base.OnSubActionEventCallRequsted();
        _ammoCountsInMagazine = _subResultStrategy.DecreaseBullet(_ammoCountsInMagazine); // 발사 시 총알 감소 적용
        OnRoundChangeRequested?.Invoke(true, _ammoCountsInMagazine, _ammoCountsInPossession);
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
}