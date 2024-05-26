using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BehaviorTree.Nodes
{
    public class Attack : Node
    {
        Transform _attackPoint;
        float _attackRadius;
        float _attackDamage;

        float _attackPreDelay;
        float _attackAfterDelay;

        LayerMask _attackLayer;
        Action<string> ResetAnimatorValue;

        StopwatchTimer _attackPreTimer;
        StopwatchTimer _attackAfterTimer;

        Func<bool> IsTargetInSight;
        Func<ISightTarget> ReturnTargetInSight;
        Action<SoundType, bool> PlaySFX;

        public Attack(Transform attackPoint, float attackDamage, float attackPreDelay, float attackAfterDelay, float attackRadius, LayerMask attackLayer
            , Action<string> ResetAnimatorValue, Func<bool> IsTargetInSight, Func<ISightTarget> ReturnTargetInSight, Action<SoundType, bool> PlaySFX)
        {
            _attackPreTimer = new StopwatchTimer();
            _attackAfterTimer = new StopwatchTimer();

            this.IsTargetInSight = IsTargetInSight;
            this.ReturnTargetInSight = ReturnTargetInSight;

            _attackPoint = attackPoint;
            _attackPreDelay = attackPreDelay;
            _attackAfterDelay = attackAfterDelay;

            _attackDamage = attackDamage;
            _attackRadius = attackRadius;
            _attackLayer = attackLayer;
            this.ResetAnimatorValue = ResetAnimatorValue;
            this.PlaySFX = PlaySFX;
        }
        public override NodeState Evaluate()
        {
            if (_attackPreTimer.CurrentState == StopwatchTimer.State.Finish)
            {
                PlaySFX(SoundType.Attack, true);

                bool isIn = IsTargetInSight();
                if (isIn == false) return NodeState.FAILURE;

                ISightTarget target = ReturnTargetInSight();
                Transform sightPoint = target.ReturnSightPoint();

                RaycastHit hit;
                Vector3 dir = (sightPoint.position - _attackPoint.position).normalized;
                Physics.Raycast(_attackPoint.position, dir, out hit, _attackRadius, _attackLayer);
                Debug.DrawRay(_attackPoint.position, dir * _attackRadius, Color.red, 10);

                if (hit.transform == null) return NodeState.FAILURE;

                IDamageable damageable = hit.transform.GetComponent<IDamageable>();
                if (damageable == null) return NodeState.FAILURE;

                damageable.GetDamage(_attackDamage);

                _attackPreTimer.Reset();
            }

            if (_attackAfterTimer.CurrentState == StopwatchTimer.State.Running) return NodeState.FAILURE;
            if (_attackAfterTimer.CurrentState == StopwatchTimer.State.Finish) _attackAfterTimer.Reset();

            ResetAnimatorValue?.Invoke("NowAttack");

            _attackPreTimer.Start(_attackPreDelay);
            _attackAfterTimer.Start(_attackAfterDelay);

            return NodeState.SUCCESS;
        }
    }
}
