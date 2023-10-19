using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObserverPattern;

abstract public class Gun : BaseWeapon//, IObserver<float>
{
    [SerializeField]
    ParticleSystem _muzzleFlash;

    [SerializeField]
    ParticleSystem _emptyCartridgeSpawner;

    protected float _penetratePower = 15;
    protected float _trajectoryLineOffset = 1.3f;

    [SerializeField]
    protected Transform _muzzle;

    [SerializeField]
    protected string _trajectoryLineEffect;

    [SerializeField]
    protected int _maxBulletCountInMagazine = 30;

    [SerializeField]
    protected string _nonPenetrateHitEffect;

    public int MaxBulletCountInMagazine { get { return _maxBulletCountInMagazine; } }

    [SerializeField]
    protected int _bulletCountInMagazine;

    public int BulletCountInMagazine { get { return _bulletCountInMagazine; } set { _bulletCountInMagazine = value; } }

    [SerializeField]
    protected int _possessingBullet;

    public int PossessingBullet { get { return _possessingBullet; } set { _possessingBullet = value; } }

    protected Animator _animator;
    public Animator Animator { get { return _animator; } }

    [SerializeField]
    protected float _reloadFinishTime;

    [SerializeField]
    protected float _reloadStateExitTime;

    [SerializeField]
    protected float _receivedBulletSpreadPower;

    int bulletCountWhenActionStart;

    public override void StoreCurrentBulletCount()
    {
        bulletCountWhenActionStart = _bulletCountInMagazine;
    }

    public override bool IsMagazineEmpty()
    {
        return bulletCountWhenActionStart != 0 && _bulletCountInMagazine == 0 && _possessingBullet != 0;
    }

    public override void Initialize(GameObject player, Transform cam, Animator ownerAnimator)
    {
        base.Initialize(player, cam, ownerAnimator);

        _animator = GetComponent<Animator>();
        _bulletCountInMagazine = _maxBulletCountInMagazine;

        MovementComponent movementComponent = player.GetComponent<MovementComponent>();
        movementComponent.OnDisplacementRequested += OnDisplacementReceived;

        // 여기에서 UI에 이밴트로 연결시키는 방식
    }

    void OnDisplacementReceived(float displacement)
    {
        _receivedBulletSpreadPower = displacement;
    }

    public override void OnReload()
    {
        _animator.Play("Reload", -1, 0);
        _ownerAnimator.Play(_weaponName + "Reload", -1, 0);
    }

    public override bool CanReload()
    {
        // 현재 보유 중인 탄환이 없거나 탄창의 총알을 소모하지 않은 경우
        if (_maxBulletCountInMagazine == _bulletCountInMagazine || _possessingBullet == 0) return false;
        else return true;
    }

    public override bool CheckNowReload()
    {
        return _bulletCountInMagazine == 0 && _possessingBullet != 0;
    }

    protected override bool CanAttack()
    {
        if (_bulletCountInMagazine > 0) return true;
        else return false;
    }

    protected int ReturnCanFireCount(int originFireCount)
    {
        if(_bulletCountInMagazine - originFireCount < 0)
        {
            return _bulletCountInMagazine;
        }

        return originFireCount;
    }

    protected void CalculateLeftBulletCount(int canFireCount)
    {
        _bulletCountInMagazine -= canFireCount;
    }

    protected void CalculateLeftBulletCount()
    {
        _bulletCountInMagazine -= 1;
    }

    protected bool Fire(ResultStrategy resultStrategy, RecoilStrategy recoilStrategy, int originFireCount)
    {
        if (CanAttack() == false) return false;

        int canFireCount = ReturnCanFireCount(originFireCount);
        CalculateLeftBulletCount(canFireCount);
        OnAttack();

        resultStrategy.Do(_receivedBulletSpreadPower, canFireCount);

        if (_bulletCountInMagazine == 0) recoilStrategy.RecoverRecoil();
        else recoilStrategy.CreateRecoil();

        return true;
    }

    protected bool Fire(ResultStrategy resultStrategy, RecoilStrategy recoilStrategy)
    {
        if (CanAttack() == false) return false;

        CalculateLeftBulletCount();
        OnAttack();

        resultStrategy.Do(_receivedBulletSpreadPower);

        if (_bulletCountInMagazine == 0) recoilStrategy.RecoverRecoil();
        else recoilStrategy.CreateRecoil();

        return true;
    }

    public override void ReloadAmmo()
    {
        int canLoadBulletCount = _maxBulletCountInMagazine - _bulletCountInMagazine;

        if (_possessingBullet >= canLoadBulletCount)
        {
            _bulletCountInMagazine = _maxBulletCountInMagazine;
            _possessingBullet -= canLoadBulletCount;
        }
        else
        {
            _bulletCountInMagazine += _possessingBullet;
            _possessingBullet = 0;
        }

        OnRoundChangeRequested(_bulletCountInMagazine, _possessingBullet);

        //NotifyToObservers(_bulletCountInMagazine, _possessingBullet);
    }

    public override float ReturnReloadFinishTime() { return _reloadFinishTime; }

    public override float ReturnReloadStateExitTime() { return _reloadStateExitTime; }

    public override void OnEquip()
    {
        base.OnEquip();
        //_bulletSpreadPowerSubject.AddObserver(_bulletSpreadPowerObserver);

        _animator.Play("Equip", -1, 0);
        OnActiveContainerRequested?.Invoke(true);
        OnRoundChangeRequested?.Invoke(_bulletCountInMagazine, _possessingBullet);

        //NotifyToObservers(_bulletCountInMagazine, _possessingBullet);
    }


    public override void OnUnEquip()
    {
        //_bulletSpreadPowerSubject.RemoveObserver(_bulletSpreadPowerObserver);
        base.OnUnEquip();
    }

    protected override void OnAttack()
    {
        _muzzleFlash.Play();
        _emptyCartridgeSpawner.Play();
        _animator.Play("Fire", -1, 0f);

        //NotifyToObservers(_bulletCountInMagazine, _possessingBullet);
        OnRoundChangeRequested?.Invoke(_bulletCountInMagazine, _possessingBullet);
    }

    protected void PlayMainActionAnimation()
    {
        _ownerAnimator.Play(_weaponName + "MainAction", -1, 0);
    }

    protected void PlaySubActionAnimation()
    {
        _ownerAnimator.Play(_weaponName + "SubAction", -1, 0);
    }


    // 마우스에 손을 때는 경우 반동 회복 시퀀스 작동
    // Create는 이밴트에 따라 작동하는 위치가 다르지만
    // Recover은 같으므로 여기에 넣자
    protected override void ChainMainActionEndEvent()
    {
        _mainRecoilGenerator.RecoverRecoil();
    }

    protected override void ChainSubActionEndEvent()
    {
        _subRecoilGenerator.RecoverRecoil();
    }
}