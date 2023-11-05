using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// ���� �׽�ũ ����
abstract public class ReloadStrategy
{
    public abstract void Reload(int ammoCountInMagazine, int ammoCountInPossession, int maxAmmoCountInMagazine);

    public abstract void OnUpdate();

    /// <summary>
    /// Ư���� ������� Reload���� Ż�� ������ ���
    /// </summary>
    public abstract bool CanReloadExit();


    /// <summary>
    /// ���ε尡 ���� ��� �ش� State���� Ż��
    /// </summary>
    public abstract bool IsReloadFinish();

    protected abstract void CalculateAmmoWhenReload();

    /// <summary>
    /// ���ε� ��� �� �۵� 
    /// </summary>
    public abstract void OnResetReload();

    public abstract void OnUnlink();

    public abstract void OnLink();

    public abstract void OnInintialize();
}

public class NoReload : ReloadStrategy
{
    public override void Reload(int ammoCountInMagazine, int ammoCountInPossession, int maxAmmoCountInMagazine) { }

    public override void OnUpdate() { }
   
    public override bool CanReloadExit() { return false; }

    protected override void CalculateAmmoWhenReload() { } // �Ѿ� ���
   
    public override bool IsReloadFinish() { return false; }
  
    public override void OnResetReload() { }

    public override void OnUnlink() { }

    public override void OnLink() { }

    public override void OnInintialize() { }
}

// źâ���� �����ϴ� ���
public class MagazineReload : ReloadStrategy
{
    Timer _reloadTimer;
    float _reloadDuration;

    Timer _reloadExitTimer;
    float _reloadExitDuration;

    int _ammoCountInMagazine;
    int _maxAmmoCountInMagazine;
    int _ammoCountsInPossession;

    string _weaponName;
    Animator _weaponAnimator;
    Animator _ownerAnimator;

    Action<int, int> OnReloadRequested;

    public MagazineReload(float reloadDuration, float reloadExitDuration, string weaponName, Animator weaponAnimator,
        Animator ownerAnimator, Action<int, int> onReloadRequested)
    {
        _reloadTimer = new Timer();
        _reloadDuration = reloadDuration;

        _reloadExitTimer = new Timer();
        _reloadExitDuration = reloadExitDuration;

        _weaponName = weaponName;

        _weaponAnimator = weaponAnimator;
        _ownerAnimator = ownerAnimator;

        OnReloadRequested = onReloadRequested;
    }

    public override bool IsReloadFinish()
    {
        return _reloadExitTimer.IsFinish;
    }

    public override bool CanReloadExit()
    {
        return false;
    }

    public override void Reload(int ammoCountInMagazine, int ammoCountInPossession, int maxAmmoCountInMagazine) 
    {
        _maxAmmoCountInMagazine = maxAmmoCountInMagazine;
        _ammoCountInMagazine = ammoCountInMagazine;
        _ammoCountsInPossession = ammoCountInPossession;

        _reloadTimer.Start(_reloadDuration);
        _reloadExitTimer.Start(_reloadExitDuration);

        _weaponAnimator.Play("Reload", -1, 0);
        _ownerAnimator.Play(_weaponName + "Reload", -1, 0);
    }

    public override void OnUpdate() 
    {
        _reloadTimer.Update();
        if(_reloadTimer.IsFinish)
        {
            CalculateAmmoWhenReload();
        }

        _reloadExitTimer.Update();
    }

    public override void OnResetReload()
    {
        _reloadTimer.Reset();
        _reloadExitTimer.Reset();
    }

    protected override void CalculateAmmoWhenReload()
    {
        int canLoadBulletCount = _maxAmmoCountInMagazine - _ammoCountInMagazine;

        if (_ammoCountsInPossession >= canLoadBulletCount)
        {
            _ammoCountInMagazine = _maxAmmoCountInMagazine;
            _ammoCountsInPossession -= canLoadBulletCount;
        }
        else
        {
            _ammoCountInMagazine += _ammoCountsInPossession;
            _ammoCountsInPossession = 0;
        }

        OnReloadRequested?.Invoke(_ammoCountInMagazine, _ammoCountsInPossession);
    }

    // ȹ���ϸ� ��������ְ� �����ϸ� ������ ������
    public override void OnUnlink()
    {
        //LeftRoundShower _leftRoundShower = GameObject.FindWithTag("BulletLeftShower").GetComponent<LeftRoundShower>();
        //OnRoundChangeRequested -= _leftRoundShower.OnBulletCountChange;
    }

    public override void OnLink()
    {
        //LeftRoundShower _leftRoundShower = GameObject.FindWithTag("BulletLeftShower").GetComponent<LeftRoundShower>();
        //OnRoundChangeRequested += _leftRoundShower.OnBulletCountChange;
    }

    public override void OnInintialize()
    {
        //LeftRoundShower _leftRoundShower = GameObject.FindWithTag("BulletLeftShower").GetComponent<LeftRoundShower>();
        //OnRoundChangeRequested += _leftRoundShower.OnBulletCountChange;
    }
}

//// �� �߾� �����ϴ� ���
public class RoundByRoundReload : ReloadStrategy
{
    Timer _reloadTimer;
    float _reloadDuration;

    Timer _reloadExitTimer;
    float _reloadExitDuration;

    int _ammoCountInMagazine;
    int _maxAmmoCountInMagazine;
    int _ammoCountsInPossession;

    string _weaponName;
    Animator _weaponAnimator;
    Animator _ownerAnimator;

    Action<int, int> OnReloadRequested;

    float _storedReloadRatio;

    public RoundByRoundReload(float reloadDuration, float reloadExitDuration, string weaponName, Animator weaponAnimator,
        Animator ownerAnimator, Action<int, int> onReloadRequested)
    {
        _reloadTimer = new Timer();
        _reloadDuration = reloadDuration;

        _reloadExitTimer = new Timer();
        _reloadExitDuration = reloadExitDuration;

        _weaponName = weaponName;

        _weaponAnimator = weaponAnimator;
        _ownerAnimator = ownerAnimator;

        OnReloadRequested = onReloadRequested;
        _storedReloadRatio = 0;
    }

    public override bool IsReloadFinish()
    {
        return _reloadExitTimer.IsFinish;
    }

    public override bool CanReloadExit()
    {
        return Input.GetMouseButtonDown(0); // ���콺 ��ư�� �������� ��� Reload State���� Ż��
    }

    public override void Reload(int ammoCountInMagazine, int ammoCountInPossession, int maxAmmoCountInMagazine)
    {
        _maxAmmoCountInMagazine = maxAmmoCountInMagazine;
        _ammoCountInMagazine = ammoCountInMagazine;
        _ammoCountsInPossession = ammoCountInPossession;

        _reloadTimer.Start(_reloadDuration);
        _reloadExitTimer.Start(_reloadExitDuration);

        _weaponAnimator.Play("Reload", -1, 0);
        _ownerAnimator.Play(_weaponName + "Reload", -1, 0);
    }

    public override void OnUpdate()
    {
        _reloadTimer.Update();

        _reloadTimer.Ratio



        if (_reloadTimer.Ratio > ) // --> Ratio�� �����ؼ� 7���� �����ؾ��ϴ� ��� 1 / 7 ���� 1�߾� �߰�����
        {
            CalculateAmmoWhenReload();
            _storedReloadRatio +
        }

        _reloadExitTimer.Update();
    }

    public override void OnResetReload()
    {
        _reloadTimer.Reset();
        _reloadExitTimer.Reset();
    }

    protected override void CalculateAmmoWhenReload()
    {
        int canLoadBulletCount = _maxAmmoCountInMagazine - _ammoCountInMagazine;

        if(canLoadBulletCount > _ammoCountInMagazine)
        {
            _ammoCountsInPossession -= 1;
            _ammoCountInMagazine += 1;

            OnReloadRequested?.Invoke(_ammoCountInMagazine, _ammoCountsInPossession);
        }
    }

    // ȹ���ϸ� ��������ְ� �����ϸ� ������ ������
    public override void OnUnlink()
    {
        //LeftRoundShower _leftRoundShower = GameObject.FindWithTag("BulletLeftShower").GetComponent<LeftRoundShower>();
        //OnRoundChangeRequested -= _leftRoundShower.OnBulletCountChange;
    }

    public override void OnLink()
    {
        //LeftRoundShower _leftRoundShower = GameObject.FindWithTag("BulletLeftShower").GetComponent<LeftRoundShower>();
        //OnRoundChangeRequested += _leftRoundShower.OnBulletCountChange;
    }

    public override void OnInintialize()
    {
        //LeftRoundShower _leftRoundShower = GameObject.FindWithTag("BulletLeftShower").GetComponent<LeftRoundShower>();
        //OnRoundChangeRequested += _leftRoundShower.OnBulletCountChange;
    }
}