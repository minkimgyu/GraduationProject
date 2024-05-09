using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 장전 테스크 적용
abstract public class ReloadStrategy : BaseStrategy
{
    public ReloadStrategy() : base() { }

    public virtual void Execute(bool isTPS, int ammoCountInMagazine, int ammoCountInPossession) { }


    /// <summary>
    /// 특수한 방법으로 Reload에서 탈출 가능한 경우
    /// </summary>
    public virtual bool CanCancelReloadingByLeftClick() { return false; }

    public virtual bool CanCancelReloadingByRightClick() { return false; }

    /// <summary>
    /// 리로드가 끝난 경우 해당 State에서 탈출
    /// </summary>
    public virtual bool IsReloadFinish() { return false; }
    //public abstract bool IsReloadRunning();

    protected virtual void CalculateAmmoWhenReload() { }

    /// <summary>
    /// 리로드 취소 시 작동 
    /// </summary>
    public virtual void OnCancelReload() { }

    //  return _ammoCountsInMagazine == 0 && _ammoCountsInPossession > 0;
    public virtual bool CanAutoReload(int ammoCountsInMagazine, int ammoCountsInPossession) { return ammoCountsInMagazine == 0 && ammoCountsInPossession > 0; }

    //  if (_ammoCountsInPossession <= 0 || _maxAmmoCountInMagazine == _ammoCountsInMagazine) return false;
    //  else return true;
    public virtual bool CanReload(int ammoCountsInMagazine, int ammoCountsInPossession, int maxAmmoCountInMagazine) 
    {
        if (ammoCountsInPossession <= 0 || maxAmmoCountInMagazine == ammoCountsInMagazine) return false;
        else return true;
    }

    //public abstract void OnUnlink();

    //public abstract void OnLink();

    //public abstract void OnInintialize();
}

public class NoReload : ReloadStrategy
{
    public NoReload() : base() { }
}

abstract public class BaseReload : ReloadStrategy
{
    protected StopwatchTimer _reloadTimer;
    protected StopwatchTimer _reloadExitTimer;

    protected float _reloadExitDuration;

    protected int _ammoCountInMagazine;
    protected int _maxAmmoCountInMagazine;
    protected int _ammoCountsInPossession;

    protected BaseWeapon.Name _weaponName;
    //protected Animator _weaponAnimator;
    //protected Animator _ownerAnimator;

    protected Action<string, int, float> OnPlayWeaponAnimation;

    protected Action<string, int, float> OnPlayOwnerAnimation;

    // 현재 무기의 총알 수를 받아오는 Func 이벤트 추가
    // 이걸로 장전 가능한지 유무 판단 진행

    protected Action<int, int> OnReloadRequested;

    public BaseReload(BaseWeapon.Name weaponName, float reloadExitDuration, int maxAmmoCountInMagazine,
        Action<string, int, float> OnPlayWeaponAnimation, Action<int, int> OnReloadRequested)
    {
        _reloadTimer = new StopwatchTimer();
        _reloadExitTimer = new StopwatchTimer();

        _maxAmmoCountInMagazine = maxAmmoCountInMagazine;

        _reloadExitDuration = reloadExitDuration;

        _weaponName = weaponName;

        this.OnPlayWeaponAnimation = OnPlayWeaponAnimation;
        this.OnReloadRequested = OnReloadRequested;
    }

    public override void LinkEvent(WeaponEventBlackboard blackboard)
    {
        OnPlayOwnerAnimation += blackboard.OnPlayOwnerAnimation;
    }

    public override void UnlinkEvent(WeaponEventBlackboard blackboard)
    {
        OnPlayOwnerAnimation -= blackboard.OnPlayOwnerAnimation;
    }

    protected void PlayAnimation(string aniName)
    {
        OnPlayWeaponAnimation?.Invoke(aniName, -1, 0);
        OnPlayOwnerAnimation?.Invoke(_weaponName.ToString() + aniName, -1, 0);
    }

    public override bool IsReloadFinish()
    {
        return _reloadExitTimer.CurrentState == StopwatchTimer.State.Finish;
    }
}

    // 탄창으로 장전하는 경우
public class MagazineReload : BaseReload
{
    protected float _reloadDuration;

    public MagazineReload(BaseWeapon.Name weaponName, float reloadDuration, float reloadExitDuration, int maxAmmoCountInMagazine,
         Action<string, int, float> OnPlayWeaponAnimation, Action<int, int> OnReloadRequested) 
        : base(weaponName, reloadExitDuration, maxAmmoCountInMagazine, OnPlayWeaponAnimation, OnReloadRequested)
    {
        _reloadDuration = reloadDuration;
    }

    public override void Execute(bool isTPS, int ammoCountInMagazine, int ammoCountInPossession) 
    {
        _ammoCountInMagazine = ammoCountInMagazine;
        _ammoCountsInPossession = ammoCountInPossession;


        _reloadTimer.Start(_reloadDuration);

        _reloadExitTimer.Reset(); // 시작 전 리셋시켜주기
        _reloadExitTimer.Start(_reloadExitDuration);

        string reloadString = "Reload";
        if (isTPS) reloadString = "TPS" + reloadString;
        PlayAnimation(reloadString);
    }

    public override void OnUpdate() 
    {
        if(_reloadTimer.CurrentState == StopwatchTimer.State.Finish)
        {
            CalculateAmmoWhenReload();
            _reloadTimer.Reset();
        }
    }

    public override void OnCancelReload()
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
    StopwatchTimer _reloadBeforeTimer;
    float _reloadBeforeDuration;

    float _reloadDurationPerRound;
    float _storedReloadRatio;
    float _reloadRatio;

    public RoundByRoundReload(BaseWeapon.Name weaponName, float reloadBeforeDuration, float reloadDurationPerRound, 
        float reloadExitDuration, int maxAmmoCountInMagazine, 
        Action<string, int, float> OnPlayWeaponAnimation, Action<int, int> OnReloadRequested)
        : base(weaponName, reloadExitDuration, maxAmmoCountInMagazine, OnPlayWeaponAnimation, OnReloadRequested)
    {
        _reloadBeforeDuration = reloadBeforeDuration;
        _reloadDurationPerRound = reloadDurationPerRound;

        _reloadBeforeTimer = new StopwatchTimer();
        _storedReloadRatio = 0;
    }

    public override bool CanCancelReloadingByLeftClick() { return Input.GetMouseButtonDown(0); }
    public override bool CanCancelReloadingByRightClick() { return Input.GetMouseButtonDown(1); }

    public override void Execute(bool isTPS, int ammoCountInMagazine, int ammoCountInPossession) // maxAmmoCountInMagazine 이건 생성자에서 받기
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
        if (_reloadBeforeTimer.CurrentState == StopwatchTimer.State.Finish)
        {
            float reloadDuration = ReturnCanReloadRoundCount() * _reloadDurationPerRound;

            _reloadTimer.Start(reloadDuration);
            _reloadExitTimer.Start(reloadDuration + 0.16f);

            PlayAnimation("FirstReload");
            _reloadBeforeTimer.Reset();
        }

        if (_reloadBeforeTimer.CurrentState == StopwatchTimer.State.Running) return;

        if (_reloadTimer.CurrentState == StopwatchTimer.State.Finish)
        {
            PlayAnimation("EndReload");

            _storedReloadRatio = 0; // 끝날 때, 초기화 시켜주자
            _reloadTimer.Reset();
        }

        if(_reloadTimer.CurrentState == StopwatchTimer.State.Running)
        {
            // --> Ratio로 구분해서 7발을 장전해야하는 경우 1 / 7 마다 1발씩 추가해줌
            if (_storedReloadRatio < _reloadTimer.Ratio)
            {
                CalculateAmmoWhenReload();
                _storedReloadRatio += _reloadRatio;
                PlayAnimation("AfterReload");
            }
        }
    }

    public override void OnCancelReload()
    {
        PlayAnimation("EndReload");

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