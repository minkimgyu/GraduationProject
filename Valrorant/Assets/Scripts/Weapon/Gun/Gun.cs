using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Gun : BaseWeapon
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
    protected int _possessingBullet = 60;

    public int PossessingBullet { get { return _possessingBullet; } set { _possessingBullet = value; } }

    protected Animator _animator;
    public Animator Animator { get { return _animator; } }

    [SerializeField]
    protected float reloadFinishTime;

    [SerializeField]
    protected float reloadStateExitTime;


    public override void Initialize(Transform cam, Animator ownerAnimator)
    {
        base.Initialize(cam, ownerAnimator);

        _animator = GetComponent<Animator>();
        _bulletCountInMagazine = _maxBulletCountInMagazine;
        // 여기에서 UI에 이밴트로 연결시키는 방식
    }

    public override void OnReload()
    {
        _animator.Play("Reload");
        _ownerAnimator.Play(_weaponName + "Reload");
    }

    public override bool CanReload()
    {
        // 현재 보유 중인 탄환이 없거나 탄창의 총알을 소모하지 않은 경우
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
    }

    public override float ReturnReloadFinishTime() { return reloadFinishTime; }

    public override float ReturnReloadStateExitTime() { return reloadStateExitTime; }

    public override void OnEquip()
    {
        base.OnEquip();
        _animator.Play("Equip");
    }

    protected override void OnAttack()
    {
        _muzzleFlash.Play();
        _emptyCartridgeSpawner.Play();
        _animator.Play("Fire", -1, 0f);
    }

    protected override void ChainMainAction()
    {
        _ownerAnimator.Play(_weaponName + "MainAction", -1, 0);
    }

    protected override void ChainSubAction()
    {
        _ownerAnimator.Play(_weaponName + "SubAction", -1, 0);
    }
}