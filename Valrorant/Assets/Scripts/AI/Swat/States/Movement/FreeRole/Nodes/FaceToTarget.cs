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

        Func<ISightTarget> ReturnTargetInSight;
        Action<Vector3> View;

        float _frontOffset = 10f;

        public FaceToTarget(Transform myTransform, Transform aimPoint, Transform sightPoint, Func<ISightTarget> ReturnTargetInSight, Action<Vector3> View)
        {
            _myTransform = myTransform;

            _aimPoint = aimPoint;
            _sightPoint = sightPoint;
            this.ReturnTargetInSight = ReturnTargetInSight;
            this.View = View;
        }

        public override NodeState Evaluate()
        {
            ISightTarget target = ReturnTargetInSight();
            if (target == null) return NodeState.FAILURE;

            Transform sightPoint = target.ReturnSightPoint();
            _aimPoint.position = sightPoint.position;

            Vector3 dir = (_aimPoint.position - _sightPoint.position).normalized;
            View?.Invoke(new Vector3(dir.x, 0, dir.z));


            _sightPoint.rotation = Quaternion.LookRotation(dir).normalized;
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