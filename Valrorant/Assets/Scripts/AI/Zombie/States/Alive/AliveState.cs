using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;
using AI.ZombieFSM;
using AI;

public class AliveState : State
{
    Action<LifeState> SetState;

    float _maxHp;
    float _hp;

    Action<float> OnHpChangeRequested;

    public AliveState(float maxHp, Action<LifeState> SetState, Action<float> OnHpChangeRequested = null)
    {
        _maxHp = maxHp;
        _hp = _maxHp;

        this.SetState = SetState;
        this.OnHpChangeRequested = OnHpChangeRequested;
    }

    public override void OnDamaged(float damage)
    {
        _hp -= damage;
        if (_hp <= 0)
        {
            _hp = 0;
            OnHpChangeRequested?.Invoke(_hp / _maxHp);
            SetState?.Invoke(LifeState.Die);
            return;
        }

        OnHpChangeRequested?.Invoke(_hp / _maxHp);
    }

    public override void OnHeal(float hpPoint)
    {
        _hp += hpPoint;
        OnHpChangeRequested?.Invoke(_hp / _maxHp);
    }
}
