using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

abstract public class ActionStrategy
{
    public Action OnActionStart;
    public Action OnActionProgress;
    public Action OnActionEnd;

    public abstract void OnMouseClickStart();
    public abstract void OnMouseClickEnd();
    public abstract void OnMouseClickProgress();

    /// <summary>
    /// Update에서 도는 함수
    /// </summary>
    public abstract void OnUpdate();
}

/// <summary>
/// 자동 액션
/// </summary>
public class AutoAttackAction : ActionStrategy
{
    float _clickDelay;
    float _clickStoredDelay = 0;
    bool _canAttack = true;

    float _attackDelay;
    float _storedDelay = 0;

    bool _isFirstAttack = false;

    public AutoAttackAction(float attackDelay)
    {
        _attackDelay = attackDelay;
        _clickDelay = attackDelay;
    }

    public override void OnMouseClickStart() 
    {
        OnActionStart();
        _isFirstAttack = true;
    }

    public override void OnMouseClickEnd()
    {
        OnActionEnd();

        _isFirstAttack = false;

        if(_canAttack == true)
        {
            _canAttack = false;
            _clickStoredDelay = 0;
        }
    }

    public override void OnUpdate()
    {
        if (_canAttack == false && _clickStoredDelay < _clickDelay)
        {
            _clickStoredDelay += Time.deltaTime;
        }
        else
        {
            _clickStoredDelay = 0;
            _canAttack = true;
        }
    }

    public override void OnMouseClickProgress()
    {
        if (_canAttack == false) return;

        if (_isFirstAttack == false && _storedDelay < _attackDelay)
        {
            _storedDelay += Time.deltaTime;
        }
        else
        {
            _storedDelay = 0;
            OnActionProgress();

            if (_isFirstAttack == true) _isFirstAttack = false;
        }
    }
}

/// <summary>
/// 수동 액션
/// </summary>
abstract public class BaseManualAction : ActionStrategy
{
    float _actionDelay;
    float _storedDelay = 0;

    bool _canAction = false;

    public BaseManualAction(float attackDelay)
    {
        _actionDelay = attackDelay;
    }

    public override void OnMouseClickStart()
    {
        if (_canAction == true)
        {
            OnActionStart();
            _canAction = false;
        }
    }

    public override void OnMouseClickEnd()
    {
        OnActionEnd();
    }

    public override void OnUpdate()
    {
        if (_canAction == false && _storedDelay < _actionDelay)
        {
            _storedDelay += Time.deltaTime;
        }
        else
        {
            _storedDelay = 0;
            _canAction = true;
        }
    }

    public override void OnMouseClickProgress()
    {
        OnActionProgress();
    }
}

public class ManualAttackAction : BaseManualAction
{
    public ManualAttackAction(float actionDelay) : base(actionDelay)
    {
    }
}

public class ManualAction : BaseManualAction
{
    public ManualAction(float actionDelay) : base(actionDelay)
    {
    }
}



/// <summary>
/// 점사
/// </summary>
//public class BurstAttactAction : ActionStrategy
//{
//    float _attackDelay;
//    float _storedDelay = 0;

//    bool _nowAttack = false;
//    bool _isFirstAttack = false;

//    int _bulletCountsInOneShoot;
//    int _storedCountsInOneShoot;

//    public BurstAttactAction(float attackDelay, int bulletCountsInOneShoot)
//    {
//        _attackDelay = attackDelay;
//        _bulletCountsInOneShoot = bulletCountsInOneShoot;
//    }

//    public override void Start()
//    {
//        if (_nowAttack == false)
//        {
//            _nowAttack = true;
//            _isFirstAttack = true;
//        }
//    }

//    public override void End()
//    {
//    }

//    public override void Tick()
//    {
//        if (_nowAttack == false) return;

//        if (_isFirstAttack == false && _storedDelay < _attackDelay)
//        {
//            _storedDelay += Time.deltaTime;
//        }
//        else
//        {
//            _storedDelay = 0;
//            OnActionStart();

//            if (_isFirstAttack == true) _isFirstAttack = false;

//            CheckStopShoot();
//        }
//    }

//    void CheckStopShoot()
//    {
//        _storedCountsInOneShoot += 1;
//        if (_storedCountsInOneShoot < _bulletCountsInOneShoot) return;

//        _nowAttack = false;
//        _isFirstAttack = false;
//        _storedCountsInOneShoot = 0;
//        _storedDelay = 0;
//    }
//}