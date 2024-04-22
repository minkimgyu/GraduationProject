using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FSM;
using Agent.Component;
using Agent.States;

namespace Agent.Controller
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] Transform _weaponParent;
        [SerializeField] float _weaponThrowPower = 3;

        BaseWeapon _nowEquipedWeapon = null;
        Dictionary<BaseWeapon.Type, BaseWeapon> _weaponsContainers = new Dictionary<BaseWeapon.Type, BaseWeapon>();

        [SerializeField] Animator _ownerAnimator;
        void PlayAnimation(string stateName, int layer, float nomalizedTime) => _ownerAnimator.Play(stateName, layer, nomalizedTime);

        WeaponEventBlackboard _eventBlackboard;

        public enum State
        {
            Idle,
            Equip,
            LeftAction,
            RightAction,
            Reload,

            Root,
            Drop
        }

        StateMachine<State> _weaponFSM;
        public Action<float> OnWeaponChangeRequested;

        private void Start()
        {
            ZoomComponent zoomComponent = GetComponent<ZoomComponent>();
            ViewComponent viewComponent = GetComponent<ViewComponent>();
            MovementComponent movementComponent = GetComponent<MovementComponent>();

            _eventBlackboard = new WeaponEventBlackboard(
                zoomComponent.OnZoomCalled,
                movementComponent.OnDisplacementRequested,
                viewComponent.OnRecoilRequested, 
                PlayAnimation,
                viewComponent.ReturnRaycastPos,
                viewComponent.ReturnRaycastDir
            );

            InitializeEvent();
            InitializeWeapons();

            _weaponFSM = new StateMachine<State>();
            InitializeFSM();
        }

        void InitializeWeapons()
        {
            BaseWeapon[] weapons = _weaponParent.GetComponentsInChildren<BaseWeapon>();
            for (int i = 0; i < weapons.Length; i++)
            {
                //weapons[i].Initialize(_eventBlackboard);
                _weaponsContainers.Add(weapons[i].WeaponType, weapons[i]);
            }
        }

        void InitializeEvent()
        {
            MovementComponent movementComponent = GetComponent<MovementComponent>();
            OnWeaponChangeRequested = movementComponent.OnWeaponChangeRequested; // 할당
        }

        /// 이벤트 모음


        Transform ReturnWeaponParent() { return _weaponParent; }
        BaseWeapon ReturnEquipedWeapon() { return _nowEquipedWeapon; }
        void ResetEquipedWeapon(BaseWeapon weapon) { _nowEquipedWeapon = weapon; }

        bool IsContainerHasSameType(BaseWeapon.Type weaponType) { return _weaponsContainers.ContainsKey(weaponType) == false; }
        void GoToEquipState(BaseWeapon.Type typeToEquip) 
        {
            if (_nowEquipedWeapon.WeaponType == typeToEquip) return; // 이미 같은 타입의 아이템이 장착되어 있으면 리턴

            _weaponFSM.SetState(State.Equip, "SendWeaponTypeToEquip", typeToEquip);
        }

        void SwitchToNewWeapon(BaseWeapon newWeapon)
        {
            if (IsContainerHasSameType(newWeapon.WeaponType))
            {
                _weaponFSM.SetState(State.Drop, "DropSameTypeWeaponAndRootNewWeapon", newWeapon);
            }
            else
            {
                _weaponFSM.SetState(State.Root, "RootNewWeapon", newWeapon);
            }
        }

        void RevertToPreviousState() => _weaponFSM.RevertToPreviousState();
        void SetState(State state) => _weaponFSM.SetState(state);
        void SetState(State state, string message, BaseWeapon newWeapon) => _weaponFSM.SetState(state, message, newWeapon);
        void SetState(State state, string message, BaseWeapon.Type weaponType) => _weaponFSM.SetState(state, message, weaponType);

        // InteractionController에서 받아온 경우
        public void OnWeaponReceived(BaseWeapon weapon) => _weaponFSM.OnWeaponReceived(weapon);

        WeaponEventBlackboard ReturnEventBlackboard() { return _eventBlackboard; }

        void RemoveWeaponInContainer(BaseWeapon.Type type) { _weaponsContainers.Remove(type); }

        void AddWeaponToContainer(BaseWeapon weapon) { _weaponsContainers.Add(weapon.WeaponType, weapon); }

        BaseWeapon ReturnSameTypeWeapon(BaseWeapon.Type type) { return _weaponsContainers[type]; }

        bool HaveSameTypeWeapon(BaseWeapon.Type type) { return _weaponsContainers.ContainsKey(type); }

        ///

        void InitializeFSM()
        {
            Dictionary<State, BaseState> weaponStates = new Dictionary<State, BaseState>();

            BaseState idle = new IdleState(SetState, GoToEquipState, SwitchToNewWeapon, ReturnEquipedWeapon);

            BaseState equip = new EquipState(SetState, SwitchToNewWeapon, ResetEquipedWeapon, ReturnSameTypeWeapon, OnWeaponChangeRequested, ReturnEquipedWeapon);

            BaseState reload = new ReloadState(SetState, GoToEquipState, SwitchToNewWeapon, ReturnEquipedWeapon);

            BaseState leftAction = new LeftActionState(SetState, ReturnEquipedWeapon);
            BaseState rightAction = new RightActionState(SetState, ReturnEquipedWeapon);

            BaseState root = new RootState(SetState, SetState, AddWeaponToContainer, ReturnEquipedWeapon, ReturnWeaponParent, ReturnEventBlackboard);
            BaseState drop = new DropState(_weaponThrowPower, RevertToPreviousState, RemoveWeaponInContainer, SetState, SetState, ReturnEquipedWeapon, HaveSameTypeWeapon,
                ReturnSameTypeWeapon, ReturnEventBlackboard);

            weaponStates.Add(State.Idle, idle);
            weaponStates.Add(State.Equip, equip);
            weaponStates.Add(State.Reload, reload);

            weaponStates.Add(State.LeftAction, leftAction);
            weaponStates.Add(State.RightAction, rightAction);

            weaponStates.Add(State.Root, root);
            weaponStates.Add(State.Drop, drop);

            _weaponFSM.Initialize(weaponStates);
            _weaponFSM.SetState(State.Equip, "EquipKnifeFirst", BaseWeapon.Type.Melee); // 초기 State 설정
        }

        private void Update()
        {
            _weaponFSM.OnUpdate();
            foreach (var weapon in _weaponsContainers) weapon.Value.OnUpdate(); // 무기 루틴 돌려주기
        }
    }
}