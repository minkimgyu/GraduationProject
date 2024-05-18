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
    float _maxArmor;

    float _hp;
    float _armor;
    const float _armorEfficiency = 0.3f;
    Action<int, int> OnShowHp;

    public AliveState(float maxHp, float maxArmor, Action<LifeState> SetState, Action<int, int> OnShowHp = null)
    {
        _maxHp = maxHp;
        _maxArmor = maxArmor;

        _hp = _maxHp;
        _armor = _maxArmor;
        this.SetState = SetState;
        this.OnShowHp = OnShowHp;
    }

    public override void OnDamaged(float damage)
    {
        if(_armor > 0)
        {
            float decreasedDamage = damage *= _armorEfficiency; // _armorEfficiency만큼 데미지를 감소시킨다.
            damage -= decreasedDamage;
            _armor -= decreasedDamage; // armor가 대신 줄어듬

            if(_armor < 0) _armor = 0;
        }

        _hp -= damage;
        if (_hp <= 0)
        {
            _hp = 0;
            SetState?.Invoke(LifeState.Die);
        }
        
        OnShowHp?.Invoke(Mathf.RoundToInt(_hp), Mathf.RoundToInt(_armor));
    }

    public override void OnHeal(float hpPoint, float armorPoint)
    {
        _armor += armorPoint;
        _hp += hpPoint;

        //if (_armor > _maxArmor) _armor = _maxArmor;
        //if (_hp > _maxHp) _hp = _maxHp;

        OnShowHp?.Invoke(Mathf.RoundToInt(_hp), Mathf.RoundToInt(_armor));
    }
}
