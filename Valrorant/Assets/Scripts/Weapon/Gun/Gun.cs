using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObserverPattern;

abstract public class Gun : BaseWeapon, IObserver<float>
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

    protected float _receivedBulletSpreadPower;

    ISubject<float> _bulletSpreadPowerSubject;
    IObserver<float> _bulletSpreadPowerObserver;

    public override void Initialize(GameObject player, Transform cam, Animator ownerAnimator)
    {
        base.Initialize(player, cam, ownerAnimator);

        _animator = GetComponent<Animator>();
        _bulletCountInMagazine = _maxBulletCountInMagazine;

        _bulletSpreadPowerSubject = _player.GetComponent<ISubject<float>>();
        _bulletSpreadPowerObserver = GetComponent<IObserver<float>>();

        // ���⿡�� UI�� �̹�Ʈ�� �����Ű�� ���
    }

    public override void OnReload()
    {
        _animator.Play("Reload", -1, 0);
        _ownerAnimator.Play(_weaponName + "Reload", -1, 0);
    }

    public override bool CanReload()
    {
        // ���� ���� ���� źȯ�� ���ų� źâ�� �Ѿ��� �Ҹ����� ���� ���
        if (_maxBulletCountInMagazine == _bulletCountInMagazine || _possessingBullet == 0) return false;
        else return true;
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

        NotifyToObservers(_bulletCountInMagazine, _possessingBullet);
    }

    public override float ReturnReloadFinishTime() { return _reloadFinishTime; }

    public override float ReturnReloadStateExitTime() { return _reloadStateExitTime; }

    public override void OnEquip()
    {
        base.OnEquip();
        _bulletSpreadPowerSubject.AddObserver(_bulletSpreadPowerObserver);

        _animator.Play("Equip", -1, 0);
        NotifyToObservers(_bulletCountInMagazine, _possessingBullet);
    }


    public override void OnUnEquip()
    {
        _bulletSpreadPowerSubject.RemoveObserver(_bulletSpreadPowerObserver);

        base.OnUnEquip();
    }

    public void Notify(float bulletSpreadPower)
    {
        _receivedBulletSpreadPower = bulletSpreadPower;
    }

    protected override void OnAttack()
    {
        _muzzleFlash.Play();
        _emptyCartridgeSpawner.Play();
        _animator.Play("Fire", -1, 0f);

        NotifyToObservers(_bulletCountInMagazine, _possessingBullet);
    }

    protected void PlayMainActionAnimation()
    {
        _ownerAnimator.Play(_weaponName + "MainAction", -1, 0);
    }

    protected void PlaySubActionAnimation()
    {
        _ownerAnimator.Play(_weaponName + "SubAction", -1, 0);
    }


    // ���콺�� ���� ���� ��� �ݵ� ȸ�� ������ �۵�
    // Create�� �̹�Ʈ�� ���� �۵��ϴ� ��ġ�� �ٸ�����
    // Recover�� �����Ƿ� ���⿡ ����
    protected override void ChainMainActionEndEvent()
    {
        _mainRecoilGenerator.RecoverRecoil();
    }

    protected override void ChainSubActionEndEvent()
    {
        _subRecoilGenerator.RecoverRecoil();
    }


    //protected virtual RecoilStrategy CreateRecoilStrategy(IObserver<Vector2, Vector2> observer, float actionDelay, RecoilMap recoilMap) { return default; }
    //protected virtual RecoilStrategy CreateRecoilStrategy(IObserver<Vector2, Vector2> observer, float actionDelay, RecoilRange recoilRange) { return default; }
}