using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;
using AI.FSM;
using AI;

public class AliveState : State
{
    public enum ActionState
    {
        Idle,
        TargetFollowing,
        NoiseTracking,
    }

    Action<Zombie.LifeState> SetState;
    float _hp;

    StateMachine<ActionState> _actionFsm = new StateMachine<ActionState>();

    public AliveState(Blackboard blackboard, Action<Zombie.LifeState> SetState)
    {
        _hp = blackboard.MaxHP;
        this.SetState = SetState;

        _actionFsm.Initialize(
            new Dictionary<ActionState, BaseState>
            {
                {ActionState.Idle, new IdleState(blackboard, (state) => {_actionFsm.SetState(state); }) },
                {ActionState.NoiseTracking, new NoiseTrackingState(blackboard, (state) => {_actionFsm.SetState(state); }) },
                {ActionState.TargetFollowing, new TargetFollowingState(blackboard, (state) => {_actionFsm.SetState(state); }) },
            }
        );

        _actionFsm.SetState(ActionState.Idle);
    }

    public override void OnStateUpdate()
    {
        _actionFsm.OnUpdate();
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

    public override void OnNoiseReceived()
    {
        _actionFsm.OnNoiseReceived();
    }
}
