using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using Grid;
using Grid.Pathfinder;
using BehaviorTree.Nodes;
using System;
using AI.Component;
using AI.ZombieFSM;

namespace AI
{
    public class Zombie : MonoBehaviour, IDamageable, ISightTarget
    {
        [SerializeField] Animator _animator;
        [SerializeField] Transform _sightPoint;

        [SerializeField] Transform _attackPoint;
        LayerMask _attackLayer;

        [SerializeField] GameObject _modelObj;
        [SerializeField] Transform _myRig;

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

        public void Initialize(ZombieData data)
        {
            MyType = TargetType.Zombie;

            _attackLayer = LayerMask.GetMask("CollisionableTarget");

            GridManager gridManager = FindObjectOfType<GridManager>();
            Pathfinder pathfinder = FindObjectOfType<Pathfinder>();

            ViewCaptureComponent viewCaptureComponent = GetComponentInChildren<ViewCaptureComponent>();
            viewCaptureComponent.Initialize(data.targetCaptureRadius, data.targetCaptureAngle);

            NoiseListener noiseListener = GetComponentInChildren<NoiseListener>();
            noiseListener.Initialize(data.noiseCaptureRadius, data.maxNoiseQueueSize, OnNoiseReceived);

            MoveComponent moveComponent = GetComponent<MoveComponent>();
            moveComponent.Initialize(_sightPoint, data.moveSpeed, (id, value) => _animator.SetFloat(id, value), (id) => _animator.GetFloat(id));


            ViewComponent viewComponent = GetComponent<ViewComponent>();
            viewComponent.Initialize(data.viewSpeed);

            RouteTrackingComponent routeTrackingComponent = GetComponent<RouteTrackingComponent>();
            routeTrackingComponent.Initialize(data.pathFindDelay, moveComponent.Move, moveComponent.Stop, viewComponent.View, pathfinder.FindPath);

            _blackboard = new ZombieBlackboard(
                data.attackDamage, data.angleOffset, data.angleChangeAmount, data.wanderOffset, data.stateChangeDelay, viewCaptureComponent.transform, transform, _sightPoint, 
                data.targetCaptureAdditiveRadius, data.additiveAttackRadius, data.attackRange, data.attackCircleRadius, data.delayForNextAttack, data.preAttackDelay, _attackLayer, 
                _attackPoint,

                routeTrackingComponent.FollowPath, viewComponent.View, moveComponent.Stop, viewCaptureComponent.IsTargetInSight, gridManager.ReturnNodePos,
                noiseListener.ClearAllNoise, noiseListener.IsQueueEmpty, noiseListener.ReturnFrontNoise, routeTrackingComponent.IsFollowingFinish, viewCaptureComponent.ModifyCaptureRadius,
                ResetAnimatorValue, ResetAnimatorValue, viewCaptureComponent.ReturnTargetInSight
            );

            _lifeFsm.Initialize(
               new Dictionary<LifeState, BaseState>
               {
                    {LifeState.Alive, new AliveState(data.maxHp, (state) => {_lifeFsm.SetState(state); }) },
                    {LifeState.Die, new DieState("ZombieRagdoll", transform, _modelObj, _myRig, data.destoryDelay, OnDieRequested) },
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

        public bool IsUntrackable()
        {
            return _lifeFsm.CurrentStateName == LifeState.Die;
        }

        void OnDieRequested(float delayForDestroy)
        {
            Invoke("DestroyMe", delayForDestroy);
        }

        void DestroyMe() => Destroy(gameObject);

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

        public Transform ReturnSightPoint()
        {
            return _sightPoint;
        }

        public Transform ReturnTransform()
        {
            return transform;
        }

        public Vector3 ReturnPos()
        {
            return transform.position;
        }
    }
}