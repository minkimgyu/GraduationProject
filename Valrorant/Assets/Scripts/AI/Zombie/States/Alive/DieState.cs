using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;

namespace AI.ZombieFSM
{
    public class DieState : State
    {
        GameObject _myGameObject;
        float _destoryDelay;
        Action<string, bool> ResetAnimatorBool;
        StopwatchTimer _stopwatchTimer = new StopwatchTimer();

        public DieState(ZombieBlackboard blackboard)
        {
            _myGameObject = blackboard.MyTrasform.gameObject;
            _destoryDelay = blackboard.DestoryDelay;

            ResetAnimatorBool = blackboard.ResetAnimatorBool;
        }

        public override void OnStateEnter()
        {
            ResetAnimatorBool?.Invoke("IsDie", true);
            _stopwatchTimer.Start(_destoryDelay);
        }

        public override void OnStateUpdate()
        {
            if(_stopwatchTimer.CurrentState == StopwatchTimer.State.Finish)
            {
                UnityEngine.Object.Destroy(_myGameObject);
            }
        }
    }
}
