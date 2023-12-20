using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 장전 테스크 적용
abstract public class ReloadStrategy
{
    public abstract void Reload(int ammoCountInMagazine, int ammoCountInPossession);

    public abstract void OnUpdate();

    /// <summary>
    /// 특수한 방법으로 Reload에서 탈출 가능한 경우
    /// </summary>
    public abstract bool CancelReloadAndGoToMainAction();

    public abstract bool CancelReloadAndGoToSubAction();

    /// <summary>
    /// 리로드가 끝난 경우 해당 State에서 탈출
    /// </summary>
    public abstract bool IsReloadFinish();
    public abstract bool IsReloadRunning();

    protected abstract void CalculateAmmoWhenReload();

    /// <summary>
    /// 리로드 취소 시 작동 
    /// </summary>
    public abstract void OnResetReload();

    //public abstract void OnUnlink();

    //public abstract void OnLink();

    //public abstract void OnInintialize();
}

public class NoReload : ReloadStrategy
{
    public override void Reload(int ammoCountInMagazine, int ammoCountInPossession) { }

    public override void OnUpdate() { }
   
    public override bool CancelReloadAndGoToMainAction() { return false; }
    public override bool CancelReloadAndGoToSubAction() { return false; }


    protected override void CalculateAmmoWhenReload() { } // 총알 계산
   
    public override bool IsReloadFinish() { return false; }
    public override bool IsReloadRunning() { return false; }

    public override void OnResetReload() { }
}

abstract public class BaseReload : ReloadStrategy
{
    protected Timer _reloadTimer;

    protected Timer _reloadExitTimer;
    protected float _reloadExitDuration;

    protected int _ammoCountInMagazine;
    protected int _maxAmmoCountInMagazine;
    protected int _ammoCountsInPossession;

    protected string _weaponName;
    protected Animator _weaponAnimator;
    protected Animator _ownerAnimator;

    protected Action<int, int> OnReloadRequested;

    public BaseReload(float reloadExitDuration, string weaponName, int maxAmmoCountInMagazine,
        Animator weaponAnimator, Animator ownerAnimator, Action<int, int> onReloadRequested)
    {
        _maxAmmoCountInMagazine = maxAmmoCountInMagazine;

        _reloadTimer = new Timer();

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

    public override bool IsReloadRunning()
    {
        return _reloadExitTimer.IsRunning;
    }
}

    // 탄창으로 장전하는 경우
public class MagazineReload : BaseReload
{
    protected float _reloadDuration;

    public MagazineReload(float reloadDuration, float reloadExitDuration, string weaponName, int maxAmmoCountInMagazine,
        Animator weaponAnimator, Animator ownerAnimator, Action<int, int> onReloadRequested) 
        : base(reloadExitDuration, weaponName, maxAmmoCountInMagazine, weaponAnimator, ownerAnimator, onReloadRequested)
    {
        _reloadDuration = reloadDuration;
    }

    public override bool CancelReloadAndGoToMainAction() { return false; }
    public override bool CancelReloadAndGoToSubAction() { return false; }

    public override void Reload(int ammoCountInMagazine, int ammoCountInPossession) 
    {
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
            _reloadTimer.Reset();
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
}

//// 한 발씩 장전하는 경우
public class RoundByRoundReload : BaseReload
{
    Timer _reloadBeforeTimer;
    float _reloadBeforeDuration;

    float _reloadDurationPerRound;
    float _storedReloadRatio;
    float _reloadRatio;

    public RoundByRoundReload(float reloadBeforeDuration, float reloadDurationPerRound, float reloadExitDuration, string weaponName, int maxAmmoCountInMagazine,
        Animator weaponAnimator, Animator ownerAnimator, Action<int, int> onReloadRequested)
        : base(reloadExitDuration, weaponName, maxAmmoCountInMagazine, weaponAnimator, ownerAnimator, onReloadRequested)
    {
        _reloadBeforeDuration = reloadBeforeDuration;
        _reloadBeforeTimer = new Timer();

        _reloadTimer = new Timer();
        _reloadDurationPerRound = reloadDurationPerRound;

        _storedReloadRatio = 0;
    }

    public override bool CancelReloadAndGoToMainAction() { return Input.GetMouseButtonDown(0); }
    public override bool CancelReloadAndGoToSubAction() { return Input.GetMouseButtonDown(1); }

    public override void Reload(int ammoCountInMagazine, int ammoCountInPossession) // maxAmmoCountInMagazine 이건 생성자에서 받기
    {
        _ammoCountInMagazine = ammoCountInMagazine;
        _ammoCountsInPossession = ammoCountInPossession;

        int roundCount = ReturnCanReloadRoundCount();
        _reloadRatio = 1.0f / roundCount;

        _reloadBeforeTimer.Start(_reloadBeforeDuration);
    }

    int ReturnCanReloadRoundCount()
    {
        int canReloadRoundCount = _maxAmmoCountInMagazine - _ammoCountInMagazine;
        int reloadRoundCount;

        if (canReloadRoundCount > _ammoCountsInPossession) reloadRoundCount = _ammoCountsInPossession;
        else reloadRoundCount = canReloadRoundCount;

        return reloadRoundCount;
    }

    public override void OnUpdate()
    {
        _reloadBeforeTimer.Update();
        _reloadExitTimer.Update();
        _reloadTimer.Update();

        if (_reloadBeforeTimer.IsFinish)
        {
            float reloadDuration = ReturnCanReloadRoundCount() * _reloadDurationPerRound;

            _reloadTimer.Start(reloadDuration);
            _reloadExitTimer.Start(reloadDuration + 0.16f);

            _weaponAnimator.Play("FirstReload", -1, 0);
            _ownerAnimator.Play(_weaponName + "FirstReload", -1, 0);
            _reloadBeforeTimer.Reset();
        }

        if (_reloadBeforeTimer.IsRunning) return;

        if (_reloadTimer.IsFinish)
        {
            _weaponAnimator.Play("EndReload", -1, 0);
            _ownerAnimator.Play(_weaponName + "EndReload", -1, 0);
            _storedReloadRatio = 0; // 끝날 때, 초기화 시켜주자
            _reloadTimer.Reset();
        }

        if(_reloadTimer.IsRunning)
        {
            // --> Ratio로 구분해서 7발을 장전해야하는 경우 1 / 7 마다 1발씩 추가해줌
            if (_storedReloadRatio < _reloadTimer.Ratio)
            {
                CalculateAmmoWhenReload();
                _storedReloadRatio += _reloadRatio;

                _weaponAnimator.Play("AfterReload", -1, 0);
                _ownerAnimator.Play(_weaponName + "AfterReload", -1, 0);
            }
        }
    }

    public override void OnResetReload()
    {
        _storedReloadRatio = 0;
        _reloadBeforeTimer.Reset();
        _reloadTimer.Reset();
        _reloadExitTimer.Reset();
    }

    protected override void CalculateAmmoWhenReload()
    {
        _ammoCountsInPossession -= 1;
        _ammoCountInMagazine += 1;

        OnReloadRequested?.Invoke(_ammoCountInMagazine, _ammoCountsInPossession);
    }
}