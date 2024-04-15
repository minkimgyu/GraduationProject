using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AI.Component;

namespace AI
{
    public class Swat : MonoBehaviour
    {
        [SerializeField] Transform _sightPoint;

        [SerializeField] Transform _aimTarget;

        [SerializeField] float _attackRadius;
        [SerializeField] float _additiveAttackRadius;

        [SerializeField] float _delayDuration;
        [SerializeField] float _targetCaptureAngle;

        SwatBlackboard swatBloackboard;
        [SerializeField] BattleFSM _attackController;

        private void Start()
        {
            Initialize();
        }

        void Initialize()
        {
            ViewCaptureComponent viewCaptureComponent = GetComponentInChildren<ViewCaptureComponent>();
            viewCaptureComponent.Initialize(_attackRadius, _targetCaptureAngle);

            Func<bool> IsTargetInSight = viewCaptureComponent.IsTargetInSight;
            Func<ITarget> ReturnTargetInSight = viewCaptureComponent.ReturnTargetInSight;
            Action<float> ModifyCaptureRadius = viewCaptureComponent.ModifyCaptureRadius;

            swatBloackboard = new SwatBlackboard(transform, _sightPoint, _aimTarget, _attackRadius,
            _additiveAttackRadius, _delayDuration, IsTargetInSight, ReturnTargetInSight, ModifyCaptureRadius);

            _attackController.Initialize(swatBloackboard);
        }

        private void Update()
        {
            _attackController.OnUpdate();
        }
    }
}