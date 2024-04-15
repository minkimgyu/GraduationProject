using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

abstract public class EventStrategy
{
    public Action OnActionStart;
    public Action OnActionProgress;
    public Action OnActionEnd;

    public Action OnEventCallRequsted;
    public Action OnEventCallFinished;

    public abstract void OnChange();

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
public class AutoAttackAction : EventStrategy
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
        if (_canAttack == false)
        {
            if (_clickStoredDelay < _clickDelay)
            {
                _clickStoredDelay += Time.deltaTime;
            }
            else
            {
                _clickStoredDelay = 0;
                _canAttack = true;
            }
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
            OnEventCallRequsted();

            if (_isFirstAttack == true) _isFirstAttack = false;
        }
    }

    public override void OnChange()
    {
    }
}

/// <summary>
/// 수동 액션
/// </summary>
abstract public class BaseManualAction : EventStrategy
{
    float _actionDelay;
    float _storedDelay = 0;

    bool _canAction = true;

    public BaseManualAction(float attackDelay)
    {
        _actionDelay = attackDelay;
    }

    public override void OnMouseClickStart()
    {
        if (_canAction == true)
        {
            OnActionStart();
            OnEventCallRequsted();
            _canAction = false;
        }
    }

    public override void OnMouseClickEnd()
    {
        OnActionEnd();
    }

    public override void OnUpdate()
    {
        if(_canAction == false)
        {
            if (_storedDelay < _actionDelay)
            {
                _storedDelay += Time.deltaTime;
            }
            else
            {
                _storedDelay = 0;
                _canAction = true;
            }
        }
    }

    public override void OnMouseClickProgress()
    {
        OnActionProgress();
    }
}

/// <summary>
/// 수동 액션
/// </summary>
public class BurstAttackAction : EventStrategy
{
    float _actionDelay;
    float _recoilRecoverDuration;


    float _storedDelay = 0;
    float _storedRecoverDuration = 0;

    bool _canRecover = true;
    bool _canAction = true;

    // 타이머 하나 굴려서 Ratio 마다 발사하게끔 제작
    // 몇 점사인지에 따라 다르게 제작
    int _fireCountInOneAction;
    float _storedRatio;

    // Action을 교체할 때, 이벤트를 호출해서 변수를 초기화 해주는 과정이 필요할 듯

    public BurstAttackAction(float attackDelay, float recoilRecoverDuration, int fireCountInOneAction)
    {
        _actionDelay = attackDelay;
        _recoilRecoverDuration = recoilRecoverDuration;
        _fireCountInOneAction = fireCountInOneAction;
    }

    public override void OnMouseClickStart()
    {
        if (_canAction == true)
        {
            OnActionStart();
            _canAction = false;

            if(_canRecover == false)
            {
                _storedRecoverDuration = 0;
            }
            else
            {
                _canRecover = false;
            }
        }
    }

    public override void OnMouseClickEnd()
    {
        OnActionEnd();
    }

    void Action()
    {
        float ratio = _storedDelay / _actionDelay; // delayRatio
        if (ratio > 1) return;

        if (_canAction == false && _storedDelay < _actionDelay && _storedRatio < ratio)
        {
            OnEventCallRequsted();
            _storedRatio += 1.0f / _fireCountInOneAction; // burstRatio
        }
    }

    public override void OnUpdate()
    {
        Action();

        if(_canRecover == false)
        {
            if (_storedRecoverDuration < _recoilRecoverDuration)
            {
                _storedRecoverDuration += Time.deltaTime;
            }
            else
            {
                _storedRecoverDuration = 0;
                _canRecover = true;
                OnEventCallFinished(); // 반동 회복 호출
            }
        }

        if(_canAction == false)
        {
            if (_storedDelay < _actionDelay)
            {
                _storedDelay += Time.deltaTime;
            }
            else
            {
                _storedDelay = 0;
                _storedRatio = 0;
                _canAction = true;
            }
        }
    }

    public override void OnMouseClickProgress()
    {
        OnActionProgress();
    }

    public override void OnChange()
    {
        _storedDelay = 0;
        _storedRatio = 0;
        _canAction = true;
    }
}

public class ManualAttackAction : BaseManualAction
{
    public ManualAttackAction(float actionDelay) : base(actionDelay)
    {
    }

    public override void OnChange()
    {
    }
}

public class ManualAction : BaseManualAction
{
    public ManualAction(float actionDelay) : base(actionDelay)
    {
    }

    public override void OnChange()
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