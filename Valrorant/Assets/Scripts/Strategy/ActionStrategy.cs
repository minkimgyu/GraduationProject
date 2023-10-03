using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

abstract public class ActionStrategy
{
    public Action OnActionStart;

    public virtual void Start() { }
    public virtual void End() { }
    public virtual void Progress() { }

    protected bool nowAction = false;

    public bool NowAction { get { return nowAction; } set { nowAction = value; } }

    /// <summary>
    /// Update에서 도는 함수
    /// </summary>
    public virtual void Tick() { }
}

/// <summary>
/// 연발 사격
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

    public override void Start() 
    {
        if (_canAttack == false) return;

        _isFirstAttack = true;
        nowAction = true;
        _canAttack = false;
    }

    public override void End()
    {
        _isFirstAttack = false;
        nowAction = false;
    }

    public override void Tick()
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

    public override void Progress()
    {
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

    public override void Start()
    {
        if (_canAttack == true)
        {
            nowAction = true;

            OnActionStart();
            _canAttack = false;

            nowAction = false;
        }
    }

    public override void End()
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