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

        Action FireEventStart;
        Action FireEventProgress;
        Action FireEventEnd;

        StopwatchTimer _delayTimer = new StopwatchTimer(); // �̰Ŵ� ���� �� ������ ǥ���� ���� Ÿ�̸�
        StopwatchTimer _fireTimer = new StopwatchTimer(); // �̰Ŵ� ���� �ð��� ǥ���ϱ� ���� Ÿ�̸�

        public AttackState(SwatBlackboard blackboard, Action<BattleFSM.State> SetState,
            Action FireEventStart, Action FireEventProgress, Action FireEventEnd)
        {
            this.SetState = SetState;
            IsTargetInSight = blackboard.IsTargetInSight;
            ModifyCaptureRadius = blackboard.ModifyCaptureRadius;

            _additiveAttackRadius = blackboard.AdditiveAttackRadius;

            ReturnTargetInSight = blackboard.ReturnTargetInSight;
            _aimTarget = blackboard.AimTarget;

            this.FireEventStart = FireEventStart;
            this.FireEventProgress = FireEventProgress;
            this.FireEventEnd = FireEventEnd;
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
            _fireTimer.Start(2);
        }

        public override void OnStateExit()
        {
            FireEventEnd?.Invoke(); // ���°� �ٲ㼭 �ѱ� ������ ������ �ϴ� ��� ȣ��
            ModifyCaptureRadius?.Invoke(-_additiveAttackRadius);
        }

        public override void OnStateUpdate()
        {
            ITarget target = ReturnTargetInSight();
            _aimTarget.position = target.ReturnPos();

            if (_delayTimer.CurrentState == StopwatchTimer.State.Running) return;
            if (_delayTimer.CurrentState == StopwatchTimer.State.Finish)
            {
                _fireTimer.Start(2);
            }

            if (_fireTimer.CurrentState == StopwatchTimer.State.Running)
            {
                FireEventProgress?.Invoke();
            }
            else if (_fireTimer.CurrentState == StopwatchTimer.State.Finish)
            {
                _delayTimer.Start(5);
            }
        }
    }
}