using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;

namespace AI.SwatFSM
{
    public class IdleState : State
    {
        Action<BattleFSM.State> SetState;
        Func<bool> IsTargetInSight;
        Transform _aimTarget;
        Transform _sightPoint;
        Transform _myTransform;

        public IdleState(SwatBlackboard blackboard, Action<BattleFSM.State> SetState)
        {
            this.SetState = SetState;
            IsTargetInSight = blackboard.IsTargetInSight;
            _aimTarget = blackboard.AimTarget;
            _sightPoint = blackboard.SightPoint;
            _myTransform = blackboard.MyTransform;
        }

        public override void CheckStateChange()
        {
            bool isInSight = IsTargetInSight();
            if (isInSight == false) return;

            SetState?.Invoke(BattleFSM.State.Attack);
        }

        public override void OnStateEnter()
        {
        }

        public override void OnStateExit()
        {
        }

        public override void OnStateUpdate()
        {
            _aimTarget.position = _sightPoint.position + _myTransform.forward;
        }
    }
}