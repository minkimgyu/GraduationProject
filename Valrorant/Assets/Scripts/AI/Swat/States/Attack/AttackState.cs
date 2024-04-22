using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;

namespace AI.SwatFSM
{
    public class AttackState : State
    {
        Action<BattleFSM.State> SetState;
        Func<bool> IsTargetInSight;
        Action<float> ModifyCaptureRadius;

        float _additiveAttackRadius;

        Func<ITarget> ReturnTargetInSight;
        Transform _aimTarget;

        public AttackState(SwatBlackboard blackboard, Action<BattleFSM.State> SetState)
        {
            this.SetState = SetState;
            IsTargetInSight = blackboard.IsTargetInSight;
            ModifyCaptureRadius = blackboard.ModifyCaptureRadius;

            _additiveAttackRadius = blackboard.AdditiveAttackRadius;

            ReturnTargetInSight = blackboard.ReturnTargetInSight;
            _aimTarget = blackboard.AimTarget;
        }

        public override void CheckStateChange()
        {
            bool isInSight = IsTargetInSight();
            if (isInSight == true) return;

            SetState?.Invoke(BattleFSM.State.Idle);
        }

        public override void OnStateEnter()
        {
            ModifyCaptureRadius?.Invoke(_additiveAttackRadius);
        }

        public override void OnStateExit()
        {
            ModifyCaptureRadius?.Invoke(-_additiveAttackRadius);
        }

        public override void OnStateUpdate()
        {
            ITarget target = ReturnTargetInSight();
            _aimTarget.position = target.ReturnPos();
        }
    }
}