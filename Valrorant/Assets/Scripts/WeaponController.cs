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
        float _weaponThrowPower;

        BaseWeapon _nowEquipedWeapon = null;
        Dictionary<BaseWeapon.Type, BaseWeapon> _weaponsContainer;

        WeaponEventBlackboard _eventBlackboard;

        public enum InputState
        {
            Enable,
            Disable
        }

        InputState _inputState;

        public void TurnOnOffInput()
        {
            switch (_inputState)
            {
                case InputState.Enable:
                    _inputState = InputState.Disable;
                    break;
                case InputState.Disable:
                    _inputState = InputState.Enable;
                    break;
            }
        }

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
        public Action<float> OnWeaponWeightChangeRequested;

        Action<BaseWeapon.Name> OnProfileChangeRequested;

        Action<BaseWeapon.Name, BaseWeapon.Type> AddPreview;
        Action<BaseWeapon.Type> RemovePreview;

        Rigidbody _rigidbody;
        public float SendMoveDisplacement() { return _rigidbody.velocity.magnitude * 0.01f; }

        bool _isTPS; // TPS 모델인 경우

        // 여기에 이밴트 넣어서 WeaponEventBlackboard 이거 하당해주자
        public void Initialize(float weaponThrowPower, bool isTPS, Action<bool, float, Vector3, float> OnZoomRequested, 
            Action<string, int, float> OnPlayOwnerAnimation, Action<bool, int, int> OnShowRounds = null, Action<BaseWeapon.Name> OnProfileChangeRequested = null,
            Action<BaseWeapon.Name, BaseWeapon.Type> AddPreview = null, Action<BaseWeapon.Type> RemovePreview = null)
        {
            this.AddPreview = AddPreview;
            this.RemovePreview = RemovePreview;
            this.OnProfileChangeRequested = OnProfileChangeRequested;

            _inputState = InputState.Enable;

            _weaponThrowPower = weaponThrowPower;
            _isTPS = isTPS;

            _weaponsContainer = new Dictionary<BaseWeapon.Type, BaseWeapon>();
            _rigidbody = GetComponent<Rigidbody>();
            RecoilReceiver recoilReceiver = GetComponent<RecoilReceiver>();

            _eventBlackboard = new WeaponEventBlackboard(
               OnZoomRequested,
               SendMoveDisplacement,
               recoilReceiver.OnRecoilRequested,
               OnPlayOwnerAnimation,
               recoilReceiver.ReturnRaycastPos,
               recoilReceiver.ReturnRaycastDir,
               OnShowRounds
           );

            InitializeWeapons();
            InitializeFSM();
        }

        public void RefillAmmo()
        {
            foreach (var weapon in _weaponsContainer)
            {
                weapon.Value.RefillAmmo();
            }
        }

        void InitializeWeapons()
        {
            WeaponPlant plant = FindObjectOfType<WeaponPlant>();

            BaseWeapon lmg = plant.Create(BaseWeapon.Name.AR);
            lmg.transform.SetParent(_weaponParent);

            lmg.OnRooting(_eventBlackboard);
            AddWeaponToContainer(lmg);

            BaseWeapon pistol = plant.Create(BaseWeapon.Name.SMG);
            pistol.transform.SetParent(_weaponParent);

            pistol.OnRooting(_eventBlackboard);
            AddWeaponToContainer(pistol);

            BaseWeapon knife = plant.Create(BaseWeapon.Name.Knife);
            knife.transform.SetParent(_weaponParent);

            knife.OnRooting(_eventBlackboard);
            AddWeaponToContainer(knife);
        }


        Transform ReturnWeaponParent() { return _weaponParent; }
        BaseWeapon ReturnEquipedWeapon() { return _nowEquipedWeapon; }

        public bool IsAmmoEmpty() 
        {
            if (_nowEquipedWeapon == null) return false;

            return _nowEquipedWeapon.IsAmmoEmpty(); 
        }

        void ResetEquipedWeapon(BaseWeapon weapon) { _nowEquipedWeapon = weapon; }

        void SwitchToNewWeapon(BaseWeapon newWeapon)
        {
            if (_weaponsContainer.ContainsKey(newWeapon.WeaponType) == true)
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

        void RemoveWeaponInContainer(BaseWeapon.Type type) 
        { 
            _weaponsContainer.Remove(type);
            RemovePreview?.Invoke(type);
        }

        void AddWeaponToContainer(BaseWeapon weapon) 
        { 
            _weaponsContainer.Add(weapon.WeaponType, weapon);
            AddPreview?.Invoke(weapon.WeaponName, weapon.WeaponType);
        }

        public BaseWeapon ReturnSameTypeWeapon(BaseWeapon.Type type) 
        {
            if (_weaponsContainer.ContainsKey(type) == false) return null;
            return _weaponsContainer[type]; 
        }

        bool HaveSameTypeWeapon(BaseWeapon.Type type) { return _weaponsContainer.ContainsKey(type); }

        ///
        public void OnHandleEquip(BaseWeapon.Type type)
        {
            if (_inputState == InputState.Disable) return;

            _weaponFSM.OnHandleEquip(type);
        }
        public void OnHandleEventStart(BaseWeapon.EventType type)
        {
            if (_inputState == InputState.Disable) return;

            _weaponFSM.OnHandleEventStart(type);
        }
        public void OnHandleEventEnd()
        {
            if (_inputState == InputState.Disable) return;

            _weaponFSM.OnHandleEventEnd();
        }

        public void OnHandleReload()
        {
            if (_inputState == InputState.Disable) return;

            _weaponFSM.OnHandleReload();
        }
        public void OnHandleDrop()
        {
            if (_inputState == InputState.Disable) return;

            _weaponFSM.OnHandleDrop();
        }

        void InitializeFSM()
        {
            _weaponFSM = new StateMachine<State>();
            Dictionary<State, BaseState> weaponStates = new Dictionary<State, BaseState>();

            BaseState idle = new IdleState(SetState, SetState, SwitchToNewWeapon, ReturnEquipedWeapon);

            BaseState equip = new EquipState(SetState, SwitchToNewWeapon, ResetEquipedWeapon, 
                ReturnSameTypeWeapon, OnWeaponWeightChangeRequested, OnProfileChangeRequested, ReturnEquipedWeapon);

            BaseState reload = new ReloadState(_isTPS, SetState, SetState, SwitchToNewWeapon, ReturnEquipedWeapon);

            BaseState leftAction = new LeftActionState(SetState, ReturnEquipedWeapon);
            BaseState rightAction = new RightActionState(SetState, ReturnEquipedWeapon);

            BaseState root = new RootState(SetState, SetState, AddWeaponToContainer, ReturnEquipedWeapon, ReturnWeaponParent, ReturnEventBlackboard);
            BaseState drop = new DropState(_weaponThrowPower, RevertToPreviousState, SetState, SetState, ReturnEquipedWeapon, 
                ResetEquipedWeapon, RemoveWeaponInContainer, HaveSameTypeWeapon, ReturnSameTypeWeapon, ReturnEventBlackboard);

            weaponStates.Add(State.Idle, idle);
            weaponStates.Add(State.Equip, equip);
            weaponStates.Add(State.Reload, reload);

            weaponStates.Add(State.LeftAction, leftAction);
            weaponStates.Add(State.RightAction, rightAction);

            weaponStates.Add(State.Root, root);
            weaponStates.Add(State.Drop, drop);

            _weaponFSM.Initialize(weaponStates);
            _weaponFSM.SetState(State.Idle);//, "EquipKnifeFirst", BaseWeapon.Type.Melee); // 초기 State 설정
        }

        public void OnUpdate()
        {
            _weaponFSM.OnUpdate();
            foreach (var weapon in _weaponsContainer) weapon.Value.OnUpdate(); // 무기 루틴 돌려주기
        }
    }
}