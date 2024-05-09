using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FSM;

namespace AI.SwatFSM
{
    public class AttackState : State
    {
        Action<Swat.BattleState> SetState;
        Func<bool> IsTargetInSight;

        Action<BaseWeapon.EventType> EventStart;
        Action EventEnd;

        StopwatchTimer _attackTimer;
        StopwatchTimer _attackDelayTimer;

        enum State
        {
            Idle,
            Delay,
            Action
        }

        State _state;

        float _attackDuration;
        float _attackDelay;

        public AttackState(Action<Swat.BattleState> SetState, SwatBattleBlackboard blackboard)
        {
            _attackDuration = blackboard.AttackDuration;
            _attackDelay = blackboard.AttackDelay;


            this.SetState = SetState;
            IsTargetInSight = blackboard.IsTargetInSight;
            EventStart = blackboard.EventStart;
            EventEnd = blackboard.EventEnd;

            _state = State.Idle;

            _attackTimer = new StopwatchTimer();
            _attackDelayTimer = new StopwatchTimer();
        }

        public override void OnStateUpdate()
        {
            switch (_state)
            {
                case State.Idle:
                    _attackTimer.Start(_attackDuration);
                    _state = State.Action;
                    EventStart?.Invoke(BaseWeapon.EventType.Main);

                    break;
                case State.Action:

                    if (_attackTimer.CurrentState != StopwatchTimer.State.Finish) break;

                    _attackTimer.Reset();
                    _attackDelayTimer.Start(_attackDelay);
                    _state = State.Delay;
                    EventEnd?.Invoke();

                    break;

                case State.Delay:

                    if (_attackDelayTimer.CurrentState != StopwatchTimer.State.Finish) break;

                    _attackDelayTimer.Reset();
                    _state = State.Idle;
                    break;
                default:
                    break;
            }

        }

        public override void CheckStateChange()
        {
            bool isInSight = IsTargetInSight();
            if (isInSight == true) return;

            _state = State.Idle;
            SetState(Swat.BattleState.Idle);
        }
    }
}