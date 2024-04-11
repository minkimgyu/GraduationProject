using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using Grid;
using Grid.Pathfinder;
using BehaviorTree.Nodes;
using System;
using AI.Component;
using AI.FSM;

namespace AI
{
    public class Zombie : MonoBehaviour, IDamageable
    {
        [SerializeField] float _maxHp;
        [SerializeField] float _destoryDelay = 5;

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
        [SerializeField] float _additiveAttackRadius = 0.3f;
        [SerializeField] float _attackRange = 1.2f;
        [SerializeField] float _attackCircleRadius = 1.5f; 
        [SerializeField] LayerMask _attackLayer;

        [SerializeField] float _delayForNextAttack = 3;
        [SerializeField] float _pathFindDelay = 0.5f;

        public enum LifeState
        {
            Alive,
            Die
        }

        StateMachine<LifeState> _lifeFsm = new StateMachine<LifeState>();

        public enum ActionState
        {
            Idle,
            TargetFollowing,
            NoiseTracking,
        }

        StateMachine<ActionState> _actionFsm = new StateMachine<ActionState>();

        public TargetType MyType { get; set; }

        ZombieBlackboard _blackboard;

        public void Initialize()
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

            _blackboard = new ZombieBlackboard(
                _angleOffset, _angleChangeAmount, _wanderOffset, _stateChangeDelay, viewCaptureComponent.transform, transform, _targetCaptureAdditiveRadius,
                _additiveAttackRadius, _attackRange, _attackCircleRadius, _delayForNextAttack, _attackLayer, _attackPoint, _destoryDelay, _maxHp,

                routeTrackingComponent.FollowPath, viewComponent.View, moveComponent.Stop, viewCaptureComponent.IsTargetInSight, gridManager.ReturnNodePos,
                noiseListener.ClearAllNoise, noiseListener.IsQueueEmpty, noiseListener.ReturnFrontNoise, routeTrackingComponent.IsFollowingFinish, viewCaptureComponent.ModifyCaptureRadius,
                ResetAnimatorValue, ResetAnimatorValue, viewCaptureComponent.ReturnTargetInSight
            );

            _lifeFsm.Initialize(
               new Dictionary<LifeState, BaseState>
               {
                    {LifeState.Alive, new AliveState(_blackboard, (state) => {_lifeFsm.SetState(state); }) },
                    {LifeState.Die, new DieState(_blackboard) },
               }
            );
            _lifeFsm.SetState(LifeState.Alive);

            _actionFsm.Initialize(
            new Dictionary<ActionState, BaseState>
            {
                {ActionState.Idle, new IdleState(_blackboard, (state) => {_actionFsm.SetState(state); }) },
                {ActionState.NoiseTracking, new NoiseTrackingState(_blackboard, (state) => {_actionFsm.SetState(state); }) },
                {ActionState.TargetFollowing, new TargetFollowingState(_blackboard, (state) => {_actionFsm.SetState(state); }) },
            }
        );

            _actionFsm.SetState(ActionState.Idle);
        }

        void ResetAnimatorValue(string triggerName) { _animator.SetTrigger(triggerName); }
        void ResetAnimatorValue(string boolName, bool value) 
        {
            if (_animator.GetBool(boolName) == value) return;
            _animator.SetBool(boolName, value); 
        }

        void OnNoiseReceived()
        {
            _actionFsm.OnNoiseReceived();
        }

        private void Update()
        {
            _lifeFsm.OnUpdate();
            if (_lifeFsm.CurrentStateName == LifeState.Die) return;

            _actionFsm.OnUpdate();
        }

        public Vector3 GetFowardVector()
        {
            return transform.forward;
        }

        public void GetDamage(float damage)
        {
            _lifeFsm.OnDamaged(damage);
        }
    }
}