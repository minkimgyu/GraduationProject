using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BehaviorTree.Nodes
{
    public class FaceToTarget : Node
    {
        Transform _myTransform;

        Transform _aimPoint;
        Transform _sightPoint;

        float _frontOffset = 10f;
        Action<Vector3> View;

        Func<bool> IsTargetInSmallSight;
        Func<ISightTarget> ReturnTargetInSmallSight;

        Func<bool> IsTargetInLargeSight;
        Func<ISightTarget> ReturnTargetInLargeSight;

        public FaceToTarget(Transform myTransform, Transform aimPoint, Transform sightPoint, Action<Vector3> View,

            Func<bool> IsTargetInSmallSight, Func<ISightTarget> ReturnTargetInSmallSight,
            Func<bool> IsTargetInLargeSight, Func<ISightTarget> ReturnTargetInLargeSight)
        {
            _myTransform = myTransform;

            _aimPoint = aimPoint;
            _sightPoint = sightPoint;
            this.View = View;

            this.IsTargetInSmallSight = IsTargetInSmallSight;
            this.ReturnTargetInSmallSight = ReturnTargetInSmallSight;

            this.IsTargetInLargeSight = IsTargetInLargeSight;
            this.ReturnTargetInLargeSight = ReturnTargetInLargeSight;
        }

        void ResetView(ISightTarget target)
        {
            Transform sightPoint = target.ReturnSightPoint();

            float distance = Vector3.Distance(sightPoint.position, _sightPoint.position);

            Vector3 nextPos;
            if (distance <= 2f)
            {
                Vector3 targetDir = (sightPoint.position - _sightPoint.position);
                nextPos = sightPoint.position + (targetDir * 2f);
            }
            else
            {
                nextPos = sightPoint.position;
            }

            _aimPoint.position = Vector3.Lerp(_aimPoint.position, nextPos, Time.deltaTime * 0.7f);

            Vector3 dir = (_aimPoint.position - _sightPoint.position).normalized;
            View?.Invoke(new Vector3(dir.x, 0, dir.z));

            _sightPoint.rotation = Quaternion.LookRotation(dir).normalized;
        }

        public override NodeState Evaluate()
        {
            bool isInSmallSight = IsTargetInSmallSight();
            ISightTarget target;

            if (isInSmallSight == true)
            {
                target = ReturnTargetInSmallSight(); // 만약 작은 시아에 적이 감지되었다면 얘로 대상을 정해준다.
                ResetView(target);
            }
            else
            {
                bool isInLargeSight = IsTargetInLargeSight();
                if (isInLargeSight == false) return NodeState.FAILURE;

                target = ReturnTargetInLargeSight();
                ResetView(target);
            }

            return NodeState.SUCCESS;
        }

        public override void OnDisableRequested()
        {
            _sightPoint.localRotation = Quaternion.identity;

            Vector3 dir = _myTransform.forward;
            _aimPoint.position = _sightPoint.position + dir * _frontOffset;
            // 빠져나갈 때 방향 초기화 해주기
        }
    }
}