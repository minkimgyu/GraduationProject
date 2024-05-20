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
        public FormationData(int index, int maxCount, Dictionary<CharacterPlant.Name, ICommandListener> listeners)
        {
            Index = index;
            MaxCount = maxCount;
            Listeners = listeners;
        }

        public int Index { get; } // 개별 인덱스
        public int MaxCount { get; } // 최대 개수
        public Dictionary<CharacterPlant.Name, ICommandListener> Listeners { get; } // 최대 개수
    }

    public class Helper : MonoBehaviour, ICommandListener, IDamageable, ISightTarget
    {
        [SerializeField] FormationData _formationData;

        [SerializeField] Transform _sightPoint;
        [SerializeField] Transform _aimPoint;

        [SerializeField] Animator _ownerAnimator;

        [SerializeField] ViewCaptureComponent _largeAreaCaptureComponent;
        [SerializeField] ViewCaptureComponent _smallAreaCaptureComponent;

        SwatMovementBlackboard _movementBloackboard;
        SwatBattleBlackboard _battleBlackboard;

        WeaponController _weaponController;
        InteractionController _interactionController;

        Action ApplyRecoil;
        [SerializeField] GameObject _modelObj;
        [SerializeField] Transform _myRig;

        public enum MovementState
        {
            FreeRole,
            BuildingFormation,
        }

        public enum BattleState
        {
            Idle,
            Attack,
            Swap
        }

        StateMachine<MovementState> _movementFsm = new StateMachine<MovementState>();
        StateMachine<BattleState> _battleFsm = new StateMachine<BattleState>();
        StateMachine<LifeState> _lifeFsm = new StateMachine<LifeState>();

        public TargetType MyType { get; set; }

        public void ResetFormationData(FormationData data) { _formationData = data; }
        FormationData ReturnFormationData() { return _formationData; }

        Action OnDisableProfileRequested;

        //private void Awake()
        //{
            
        //}

        public void Initialize(HelperData data, Func<Vector3> ReturnPlayerPos, Action<BaseWeapon.Name> OnWeaponProfileChangeRequested,
            Action<float> OnHpChangeRequested, Action OnDisableProfileRequested)
        {
            this.OnDisableProfileRequested = OnDisableProfileRequested;

            MyType = TargetType.Human;

            _lifeFsm.Initialize(
              new Dictionary<LifeState, BaseState>
              {
                    {LifeState.Alive, new AliveState(data.maxHp, (state) => {_lifeFsm.SetState(state); }, OnHpChangeRequested) },
                    {LifeState.Die, new DieState(null, data.ragdollName, transform, _modelObj, _myRig, data.destoryDelay, OnDieRequested) },
              }
           );
            _lifeFsm.SetState(LifeState.Alive);

            GridManager gridManager = FindObjectOfType<GridManager>();
            Pathfinder pathfinder = FindObjectOfType<Pathfinder>();

            _largeAreaCaptureComponent.Initialize(data.largeTargetCaptureRadius, data.largeTargetCaptureAngle);
            _smallAreaCaptureComponent.Initialize(data.smallTargetCaptureRadius, data.smallTargetCaptureAngle);


            MoveComponent moveComponent = GetComponent<MoveComponent>();
            moveComponent.Initialize(_sightPoint, data.moveSpeed, (id, value) => _ownerAnimator.SetFloat(id, value), (id) => _ownerAnimator.GetFloat(id));

            ViewComponent viewComponent = GetComponent<ViewComponent>();
            viewComponent.Initialize(data.viewSpeed);

            ApplyRecoil = viewComponent.ResetCamera;

            RouteTrackingComponent routeTrackingComponent = GetComponent<RouteTrackingComponent>();
            routeTrackingComponent.Initialize(data.pathFindDelay, moveComponent.Move, moveComponent.Stop, viewComponent.View, pathfinder.FindPath);

            Func<bool> IsTargetInLargetSight = _largeAreaCaptureComponent.IsTargetInSight;
            Func<ISightTarget> ReturnTargetInLargeSight = _largeAreaCaptureComponent.ReturnTargetInSight;
            Action<float> ModifyLargeCaptureRadius = _largeAreaCaptureComponent.ModifyCaptureRadius;

            Func<bool> IsTargetInSmallSight = _smallAreaCaptureComponent.IsTargetInSight;
            Func<ISightTarget> ReturnTargetInSmallSight = _smallAreaCaptureComponent.ReturnTargetInSight;
            Action<float> ModifySmallCaptureRadius = _smallAreaCaptureComponent.ModifyCaptureRadius;

            _movementBloackboard = new SwatMovementBlackboard(
                data.angleOffset, data.angleChangeAmount, data.wanderOffset, data.stateChangeDelay,
                _largeAreaCaptureComponent.transform, transform, _sightPoint, _aimPoint,
                data.farFromPlayerDistance, data.farFromPlayerDistanceOffset, data.closeDistance, data.closeDistanceOffset,
                data.farFromTargetDistance, data.farFromTargetDistanceOffset, data.formationRadius, data.formationOffset, data.formationOffsetChangeDuration,

                routeTrackingComponent.FollowPath, viewComponent.View, moveComponent.Stop,
                gridManager.ReturnNodePos, ReturnPlayerPos, ReturnFormationData,


                _largeAreaCaptureComponent.ReturnAllTargets,
                IsTargetInLargetSight, ReturnTargetInLargeSight, ModifyLargeCaptureRadius,
                IsTargetInSmallSight, ReturnTargetInSmallSight, ModifySmallCaptureRadius);

            _movementFsm.Initialize(
               new Dictionary<MovementState, BaseState>
               {
                    {MovementState.FreeRole, new FreeRoleState((state) => _movementFsm.SetState(state), _movementBloackboard) },
                    {MovementState.BuildingFormation, new BuildingFormationState((state) => _movementFsm.SetState(state),_movementBloackboard) },
               }
            );
            _movementFsm.SetState(MovementState.FreeRole);


            Action<BaseWeapon.Name, BaseWeapon.Type> AddWeaponPreview;
            Action<BaseWeapon.Type> RemoveWeaponPreview;

            Shop shop = FindObjectOfType<Shop>();
            shop.AddProfileViewer(data.personName, out AddWeaponPreview, out RemoveWeaponPreview);

            _weaponController = GetComponent<WeaponController>();
            _weaponController.Initialize(
                data.weaponThrowPower, 
                true, 
                null, 
                PlayAnimation, 
                null, 
                OnWeaponProfileChangeRequested,
                AddWeaponPreview,
                RemoveWeaponPreview
            );

            _interactionController = GetComponentInChildren<InteractionController>();
            _interactionController.Initialize();

            _battleBlackboard = new SwatBattleBlackboard(data.attackDuration, data.attackDelay, IsTargetInLargetSight, ReturnTargetInLargeSight, _weaponController.OnHandleEquip,
                _weaponController.OnHandleEventStart, _weaponController.OnHandleEventEnd, _weaponController.IsAmmoEmpty, _weaponController.ReturnSameTypeWeapon);

            _battleFsm.Initialize(
              new Dictionary<BattleState, BaseState>
              {
                    {BattleState.Idle, new IdleState((state)=>_battleFsm.SetState(state), _battleBlackboard) },
                    {BattleState.Attack, new AttackState((state)=>_battleFsm.SetState(state),_battleBlackboard) },
              }
           );
            _battleFsm.SetState(BattleState.Idle);
        }

        public bool IsUntrackable()
        {
            return _lifeFsm.CurrentStateName == LifeState.Die;
        }

        void OnDieRequested(float delayForDestroy)
        {
            OnDisableProfileRequested?.Invoke();
            Invoke("DestroyMe", delayForDestroy);
        }

        void DestroyMe() => Destroy(gameObject);

        void PlayAnimation(string name, int layer, float nomalizedTime)
        {
            //print(name);
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

        public void GoToBuildFormationState()
        {
            _movementFsm.OnHandleBuildFormation();
        }

        public void GoToFreeRoleState()
        {
            _movementFsm.OnHandleFreeRole();
        }

        public Vector3 ReturnPos()
        {
            return transform.position;
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

        public void ReceiveWeapon(BaseWeapon weapon)
        {
            _weaponController.OnWeaponReceived(weapon);
        }

        public void Heal(float hpRatio)
        {
            _lifeFsm.OnHeal(hpRatio);
        }

        public void RefillAmmo()
        {
            _weaponController.RefillAmmo();
        }

        public void ResetPos(Vector3 pos)
        {
            transform.position += pos;
        }
    }
}