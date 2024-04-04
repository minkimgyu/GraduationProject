using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using Grid;
using Grid.Pathfinder;
using BehaviorTree.Nodes;
using System;

namespace AI
{
    public class Zombie : MonoBehaviour
    {
        [SerializeField] Animator _animator;
        [SerializeField] float _angleOffset;
        [SerializeField] float _angleChangeAmount;
        [SerializeField] float _stateChangeDelay;

        [SerializeField] float _flockingMoveSpeed = 3;
        [SerializeField] float _moveSpeed = 3;
        [SerializeField] float _viewSpeed = 5;
        [SerializeField] int _wanderOffset = 7;

        [SerializeField] float _flockingCaptureRadius = 5;

        [SerializeField] float _targetCaptureRadius = 8;
        [SerializeField] float _targetCaptureAdditiveRadius = 1f;
        [SerializeField] float _targetCaptureAngle = 90;

        [SerializeField] float _noiseCaptureRadius = 11;
        [SerializeField] int _maxNoiseQueueSize = 3;

        [SerializeField] Transform _attackPoint;
        [SerializeField] float _attackRadius = 1f;
        [SerializeField] float _canAttackRange = 1.5f;
        [SerializeField] LayerMask _attackLayer;

        [SerializeField] float _delayForNextAttack = 3;
        [SerializeField] float _pathFindDelay = 0.5f;

        public enum State
        {
            Idle,
            TargetFollowing,
            NoiseTracking
        }

        StateMachine<State> _fsm = new StateMachine<State>();

        public TargetType MyType { get; set; }

        private void Start() => Invoke("Initialize", 1);

        void Initialize()
        {
            MyType = TargetType.Zombie;

            GridManager gridManager = FindObjectOfType<GridManager>();
            Pathfinder pathfinder = FindObjectOfType<Pathfinder>();

            ViewCaptureComponent viewCaptureComponent = GetComponentInChildren<ViewCaptureComponent>();
            viewCaptureComponent.Initialize(_targetCaptureRadius, _targetCaptureAngle);

            NoiseListener noiseListener = GetComponentInChildren<NoiseListener>();
            noiseListener.Initialize(_noiseCaptureRadius, _maxNoiseQueueSize, OnNoiseReceived);

            MoveComponent moveComponent = GetComponent<MoveComponent>();
            moveComponent.Initialize(_moveSpeed, ResetAnimatorValue);


            ViewComponent viewComponent = GetComponent<ViewComponent>();
            viewComponent.Initialize(_viewSpeed);

            RouteTrackingComponent routeTrackingComponent = GetComponent<RouteTrackingComponent>();
            routeTrackingComponent.Initialize(_pathFindDelay, moveComponent.Move, moveComponent.Stop, viewComponent.View, pathfinder.FindPath);

            IdleStateParameter idleStateParameter = new IdleStateParameter(
                 _angleOffset, _angleChangeAmount, _wanderOffset, _stateChangeDelay, viewCaptureComponent.transform, transform, SetState,
                 routeTrackingComponent.FollowPath, viewComponent.View, moveComponent.Stop, viewCaptureComponent.IsTargetInSight, gridManager.ReturnNodePos);

            NoiseTrackingStateParameter noiseTrackingStateParameter = new NoiseTrackingStateParameter(
                SetState, noiseListener.ClearAllNoise, routeTrackingComponent.FollowPath, viewCaptureComponent.IsTargetInSight,
                noiseListener.IsQueueEmpty, noiseListener.ReturnFrontNoise, routeTrackingComponent.IsFollowingFinish);

            TargetFollowingStateParameter targetFollowingStateParameter = new TargetFollowingStateParameter(
                _targetCaptureAdditiveRadius, _canAttackRange, _delayForNextAttack, _canAttackRange, _flockingMoveSpeed, _attackLayer,
                _attackPoint, transform, SetState, viewCaptureComponent.ModifyCaptureRadius, routeTrackingComponent.FollowPath,
                viewComponent.View, moveComponent.Move, moveComponent.Stop, ResetAnimatorValue, viewCaptureComponent.IsTargetInSight
                , viewCaptureComponent.ReturnTargetInSight);

            Dictionary<State, BaseState> states = new Dictionary<State, BaseState>
            {
                {State.Idle, new IdleState(idleStateParameter) },
                {State.NoiseTracking, new NoiseTrackingState(noiseTrackingStateParameter) },
                {State.TargetFollowing, new TargetFollowingState(targetFollowingStateParameter) }
            };

            _fsm.Initialize(states);
            SetState(State.Idle);
        }

        void ResetAnimatorValue(string triggerName) { _animator.SetTrigger(triggerName); }
        void ResetAnimatorValue(string boolName, bool value) 
        {
            if (_animator.GetBool(boolName) == value) return;
            _animator.SetBool(boolName, value); 
        }

        void SetState(State state)
        {
            _fsm.SetState(state);
        }
       
        void OnNoiseReceived()
        {
            _fsm.OnNoiseReceived();
        }

        private void Update()
        {
            if (_fsm == null) return;
            _fsm.OnUpdate();
        }
    }
}