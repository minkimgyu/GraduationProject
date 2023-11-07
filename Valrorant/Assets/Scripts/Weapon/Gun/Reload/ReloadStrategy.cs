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

    protected abstract void CalculateAmmoWhenReload();

    /// <summary>
    /// 리로드 취소 시 작동 
    /// </summary>
    public abstract void OnResetReload();

    public abstract void OnUnlink();

    public abstract void OnLink();

    public abstract void OnInintialize();
}

public class NoReload : ReloadStrategy
{
    public override void Reload(int ammoCountInMagazine, int ammoCountInPossession) { }

    public override void OnUpdate() { }
   
    public override bool CancelReloadAndGoToMainAction() { return false; }
    public override bool CancelReloadAndGoToSubAction() { return false; }


    protected override void CalculateAmmoWhenReload() { } // 총알 계산
   
    public override bool IsReloadFinish() { return false; }
  
    public override void OnResetReload() { }

    public override void OnUnlink() { }

    public override void OnLink() { }

    public override void OnInintialize() { }
}

// 탄창으로 장전하는 경우
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

    public MagazineReload(float reloadDuration, float reloadExitDuration, string weaponName, int maxAmmoCountInMagazine,
        Animator weaponAnimator, Animator ownerAnimator, Action<int, int> onReloadRequested)
    {
        _maxAmmoCountInMagazine = maxAmmoCountInMagazine;

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

    // 획득하면 연결시켜주고 해제하면 연결을 끊어줌
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

//// 한 발씩 장전하는 경우
public class RoundByRoundReload : ReloadStrategy
{
    Timer _reloadBeforeTimer;
    float _reloadBeforeDuration;

    Timer _reloadTimer;
    float _reloadDurationPerRound;

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
    float _reloadRatio;

    public RoundByRoundReload(float reloadBeforeDuration, float reloadDurationPerRound, float reloadExitDuration, string weaponName, int maxAmmoCountInMagazine,
        Animator weaponAnimator, Animator ownerAnimator, Action<int, int> onReloadRequested)
    {
        _maxAmmoCountInMagazine = maxAmmoCountInMagazine;

        _reloadBeforeDuration = reloadBeforeDuration;
        _reloadBeforeTimer = new Timer();

        _reloadTimer = new Timer();
        _reloadDurationPerRound = reloadDurationPerRound;

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

    // 획득하면 연결시켜주고 해제하면 연결을 끊어줌
    public override void OnUnlink()
    {
        //LeftRoundShower _leftRoundShower = GameObject.FindWithTag("BulletLeftShower").GetComponent<LeftRoundShower>();
        //OnReloadRequested -= _leftRoundShower.OnRoundCountChange;
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