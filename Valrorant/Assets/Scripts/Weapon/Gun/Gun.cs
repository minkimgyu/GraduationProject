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

    WaitForSeconds waitReloadFinishTime;

    protected override void Awake()
    {
        base.Awake();
        _animator = GetComponent<Animator>();
        _bulletCountInMagazine = _maxBulletCountInMagazine;
        waitReloadFinishTime = new WaitForSeconds(reloadFinishTime);
    }

    public override void Initialize(Transform holder, Transform cam, Animator ownerAnimator)
    {
        base.Initialize(holder, cam, ownerAnimator);

        // 여기에서 UI에 이밴트로 연결시키는 방식
    }

    public override void OnReload()
    {
        if (stopOtherAction == true) return;

        if (CanReload() == false) return;

        _animator.Play("Reload");
        _ownerAnimator.Play(_weaponName + "Reload");

        stopOtherActionCoroutine = StartCoroutine(EquipFinishCoroutine(waitReloadFinishTime));
    }

    bool CanReload()
    {
        // 현재 보유 중인 탄환이 없거나 탄창의 총알을 소모하지 않은 경우
        if (_maxBulletCountInMagazine == _bulletCountInMagazine || _possessingBullet == 0) return false;
        else return true;
    }

    void ReloadAmmo()
    {
        if (CanReload() == false) return;

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
}