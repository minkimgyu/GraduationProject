using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

abstract public class ActionStrategy
{
    public Action OnActionStart;

    public abstract void DoAction();
    public abstract void StopAction();
    public abstract void Tick();
}

/// <summary>
/// 연발 사격
/// </summary>
public class AutoAttackAction : ActionStrategy
{
    float _oneClickAttackDelay;
    float _onClickStoredDelay = 0;
    bool _nowCanClick = true;

    float _attackDelay;
    float _storedDelay = 0;

    bool _nowAttack = false;
    bool _isFirstAttack = false;

    public AutoAttackAction(float attackDelay)
    {
        _attackDelay = attackDelay;
        _oneClickAttackDelay = attackDelay;
    }

    public override void DoAction()
    {
        if (_nowCanClick == false) return;

        _nowCanClick = false;

        if (_nowAttack == false)
        {
            _nowAttack = true;
            _isFirstAttack = true;
        }
    }

    public override void StopAction()
    {
        if (_nowAttack == true)
        {
            _nowAttack = false;
            _isFirstAttack = false;
            _storedDelay = 0;
        }
    }

    public override void Tick()
    {
        if (_nowCanClick == false && _onClickStoredDelay < _oneClickAttackDelay)
        {
            _onClickStoredDelay += Time.deltaTime;
        }
        else
        {
            _onClickStoredDelay = 0;
            _nowCanClick = true;
        }

        if (_nowAttack == false) return;

        if (_isFirstAttack == false && _storedDelay < _attackDelay)
        {
            _storedDelay += Time.deltaTime;
        }
        else
        {
            _storedDelay = 0;
            OnActionStart();

            if (_isFirstAttack == true) _isFirstAttack = false;
        }
    }
}

/// <summary>
/// 점사
/// </summary>
public class BurstAttactAction : ActionStrategy
{
    float _attackDelay;
    float _storedDelay = 0;

    bool _nowAttack = false;
    bool _isFirstAttack = false;

    int _bulletCountsInOneShoot;
    int _storedCountsInOneShoot;

    public BurstAttactAction(float attackDelay, int bulletCountsInOneShoot)
    {
        _attackDelay = attackDelay;
        _bulletCountsInOneShoot = bulletCountsInOneShoot;
    }

    public override void DoAction()
    {
        if (_nowAttack == false)
        {
            _nowAttack = true;
            _isFirstAttack = true;
        }
    }

    public override void StopAction()
    {
    }

    public override void Tick()
    {
        if (_nowAttack == false) return;

        if (_isFirstAttack == false && _storedDelay < _attackDelay)
        {
            _storedDelay += Time.deltaTime;
        }
        else
        {
            _storedDelay = 0;
            OnActionStart();

            if (_isFirstAttack == true) _isFirstAttack = false;

            CheckStopShoot();
        }
    }

    void CheckStopShoot()
    {
        _storedCountsInOneShoot += 1;
        if (_storedCountsInOneShoot < _bulletCountsInOneShoot) return;

        _nowAttack = false;
        _isFirstAttack = false;
        _storedCountsInOneShoot = 0;
        _storedDelay = 0;
    }
}

/// <summary>
/// 단발 사격
/// </summary>
public class SingleAttactAction : ActionStrategy
{
    float _attackDelay;
    float _storedDelay = 0;

    bool _canAttack = false;

    public SingleAttactAction(float attackDelay)
    {
        _attackDelay = attackDelay;
    }

    public override void DoAction()
    {
        if (_canAttack == true)
        {
            OnActionStart();
            _canAttack = false;
        }
    }

    public override void StopAction()
    {
    }

    public override void Tick()
    {
        if (_canAttack == false && _storedDelay < _attackDelay)
        {
            _storedDelay += Time.deltaTime;
        }
        else
        {
            _storedDelay = 0;
            _canAttack = true;
        }
    }
}