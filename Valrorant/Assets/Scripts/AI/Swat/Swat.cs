using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FSM;
using AI.SwatFSM;
using Grid;
using Grid.Pathfinder;
using AI.Component;
using Agent.Controller;

namespace AI
{

    public struct FormationData
    {
        public FormationData(int index, int maxCount)
        {
            Index = index;
            MaxCount = maxCount;
        }

        public int Index { get; } // 개별 인덱스
        public int MaxCount { get; } // 최대 개수
    }

    public class Swat : MonoBehaviour, ICommandListener
    {
        [SerializeField] float _angleOffset = 90;
        [SerializeField] float _angleChangeAmount = 0.01f;

        [SerializeField] int _wanderOffset = 5;
        [SerializeField] float _stateChangeDelay = 5;
        [SerializeField] Transform _sightPoint;
        [SerializeField] Transform _aimPoint;

        [SerializeField] float _moveSpeed = 3;
        [SerializeField] float _viewSpeed = 5;

        [SerializeField] float _targetCaptureRadius = 8;
        [SerializeField] float _targetCaptureAngle = 90;

        [SerializeField] float _pathFindDelay = 0.5f;

        [SerializeField] float _farFromPlayerDistance = 9;
        [SerializeField] float _farFromPlayerDistanceOffset = 6;

        [SerializeField] float _farFromTargetDistance = 6;
        [SerializeField] float _farFromTargetDistanceOffset = 1;

        [SerializeField] float _closeDistance = 3;
        [SerializeField] float _closeDistanceOffset = 1;

        [SerializeField] float _attackDelay = 3;
        [SerializeField] float _attackDuration = 1;

        [SerializeField] float _formationRadius = 10;

        [SerializeField] Animator _ownerAnimator;
        [SerializeField] Player _player;

        SwatMovementBlackboard _movementBloackboard;
        SwatBattleBlackboard _battleBlackboard;

        WeaponController _weaponController;
        InteractionController _interactionController;

        Action ApplyRecoil;

        public enum MovementState
        {
            FreeRole,
            BuildingFormation,
        }

        public enum BattleState
        {
            Idle,
            Attack,
            Root
        }

        StateMachine<MovementState> _movementFsm = new StateMachine<MovementState>();
        StateMachine<BattleState> _battleFsm = new StateMachine<BattleState>();

        private void Start()
        {
            Invoke("Initialize", 2f);
        }

        ISightTarget ReturnPlayer() { return _player; }

        void Initialize()
        {
            GridManager gridManager = FindObjectOfType<GridManager>();
            Pathfinder pathfinder = FindObjectOfType<Pathfinder>();

            ViewCaptureComponent viewCaptureComponent = GetComponentInChildren<ViewCaptureComponent>();
            viewCaptureComponent.Initialize(_targetCaptureRadius, _targetCaptureAngle);

            MoveComponent moveComponent = GetComponent<MoveComponent>();
            moveComponent.Initialize(_moveSpeed, null);

            ViewComponent viewComponent = GetComponent<ViewComponent>();
            viewComponent.Initialize(_viewSpeed);

            ApplyRecoil = viewComponent.ResetCamera;

            RouteTrackingComponent routeTrackingComponent = GetComponent<RouteTrackingComponent>();
            routeTrackingComponent.Initialize(_pathFindDelay, moveComponent.Move, moveComponent.Stop, viewComponent.View, pathfinder.FindPath);

            Func<bool> IsTargetInSight = viewCaptureComponent.IsTargetInSight;
            Func<ISightTarget> ReturnTargetInSight = viewCaptureComponent.ReturnTargetInSight;
            Action<float> ModifyCaptureRadius = viewCaptureComponent.ModifyCaptureRadius;

            _movementBloackboard = new SwatMovementBlackboard(
                _angleOffset, _angleChangeAmount, _wanderOffset, _stateChangeDelay,
                viewCaptureComponent.transform, transform, _sightPoint, _aimPoint,
                _farFromPlayerDistance, _farFromPlayerDistanceOffset, _closeDistance, _closeDistanceOffset,
                _farFromTargetDistance, _farFromTargetDistanceOffset, _formationRadius,

                routeTrackingComponent.FollowPath, viewComponent.View, moveComponent.Stop, 
                gridManager.ReturnNodePos, ReturnPlayer, IsTargetInSight, ReturnTargetInSight);

            _movementFsm.Initialize(
               new Dictionary<MovementState, BaseState>
               {
                    {MovementState.FreeRole, new FreeRoleState((state, message, data) => _movementFsm.SetState(state, message, data), _movementBloackboard) },
                    {MovementState.BuildingFormation, new BuildingFormationState((state) => _movementFsm.SetState(state),_movementBloackboard) },
               }
            );
            _movementFsm.SetState(MovementState.FreeRole);


            _weaponController = GetComponent<WeaponController>();
            _weaponController.Initialize(true, null, PlayAnimation);

            _interactionController = GetComponentInChildren<InteractionController>();
            _interactionController.Initialize();

            _battleBlackboard = new SwatBattleBlackboard(_attackDuration, _attackDelay, IsTargetInSight, ReturnTargetInSight, _weaponController.OnHandleEquip, 
                _weaponController.OnHandleEventStart, _weaponController.OnHandleEventEnd);

            _battleFsm.Initialize(
              new Dictionary<BattleState, BaseState>
              {
                    {BattleState.Idle, new IdleState((state)=>_battleFsm.SetState(state), _battleBlackboard) },
                    {BattleState.Attack, new AttackState((state)=>_battleFsm.SetState(state),_battleBlackboard) },
                    //{BattleState.Root, new RootState((state)=>_battleFsm.SetState(state),_battleBlackboard) },
              }
           );
            _battleFsm.SetState(BattleState.Idle);
        }

        void PlayAnimation(string name, int layer, float nomalizedTime)
        {
            print(name);
            _ownerAnimator.Play(name, 2, nomalizedTime);
        }

        private void Update()
        {
            if (_weaponController != null) _weaponController.OnUpdate();

            _movementFsm.OnUpdate();
            _battleFsm.OnUpdate();
        }

        private void LateUpdate()
        {
            ApplyRecoil?.Invoke();
        }

        public void GoToBuildFormationState(FormationData data)
        {
            _movementFsm.OnHandleBuildFormation(data);
        }

        public void GoToFreeRoleState()
        {
            _movementFsm.OnHandleFreeRole();
        }
    }
}