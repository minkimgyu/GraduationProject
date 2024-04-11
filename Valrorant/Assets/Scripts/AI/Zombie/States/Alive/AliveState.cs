using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;
using AI.FSM;
using AI;

public class AliveState : State
{
    Action<Zombie.LifeState> SetState;
    float _hp;

    public AliveState(ZombieBlackboard blackboard, Action<Zombie.LifeState> SetState)
    {
        _hp = blackboard.MaxHP;
        this.SetState = SetState;
    }

    public override void OnDamaged(float damage)
    {
        _hp -= damage;
        if (_hp <= 0)
        {
            _hp = 0;
            SetState?.Invoke(Zombie.LifeState.Die);
        }
    }
}
