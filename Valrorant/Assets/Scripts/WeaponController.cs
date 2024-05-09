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
        Dictionary<BaseWeapon.Type, BaseWeapon> _weaponsContainer;

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

        Rigidbody _rigidbody;
        public float SendMoveDisplacement() { return _rigidbody.velocity.magnitude * 0.01f; }

        bool _isTPS; // TPS 모델인 경우

        // 여기에 이밴트 넣어서 WeaponEventBlackboard 이거 하당해주자
        public void Initialize(bool isTPS, Action<bool, float, Vector3, float> OnZoomRequested, Action<string, int, float> OnPlayOwnerAnimation)
        {
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
               recoilReceiver.ReturnRaycastDir
           );

            InitializeWeapons();
            InitializeFSM();
        }

        void InitializeWeapons()
        {
            WeaponPlant weaponPlant = FindObjectOfType<WeaponPlant>();


            BaseWeapon ak = weaponPlant.CreateWeapon(BaseWeapon.Name.LMG);
            ak.transform.SetParent(_weaponParent);
            ak.transform.localPosition = Vector3.zero;
            ak.transform.localRotation = Quaternion.identity;
            ak.transform.localScale = Vector3.one;

            ak.OnRooting(_eventBlackboard);
            ak.gameObject.SetActive(false);
            _weaponsContainer.Add(ak.WeaponType, ak);



            BaseWeapon pistol = weaponPlant.CreateWeapon(BaseWeapon.Name.Pistol);
            pistol.transform.SetParent(_weaponParent);
            pistol.transform.localPosition = Vector3.zero;
            pistol.transform.localRotation = Quaternion.identity;
            pistol.transform.localScale = Vector3.one;


            pistol.OnRooting(_eventBlackboard);
            pistol.gameObject.SetActive(false);
            _weaponsContainer.Add(pistol.WeaponType, pistol);




            BaseWeapon knife = weaponPlant.CreateWeapon(BaseWeapon.Name.Knife);
            knife.transform.SetParent(_weaponParent);
            knife.transform.localPosition = Vector3.zero;
            knife.transform.localRotation = Quaternion.identity;
            knife.transform.localScale = Vector3.one;

            knife.OnRooting(_eventBlackboard);
            knife.gameObject.SetActive(false);

            _weaponsContainer.Add(knife.WeaponType, knife);
        }


        Transform ReturnWeaponParent() { return _weaponParent; }
        BaseWeapon ReturnEquipedWeapon() { return _nowEquipedWeapon; }
        void ResetEquipedWeapon(BaseWeapon weapon) { _nowEquipedWeapon = weapon; }

        bool IsContainerHasSameType(BaseWeapon.Type weaponType) { return _weaponsContainer.ContainsKey(weaponType) == false; }

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

        void RemoveWeaponInContainer(BaseWeapon.Type type) { _weaponsContainer.Remove(type); }

        void AddWeaponToContainer(BaseWeapon weapon) { _weaponsContainer.Add(weapon.WeaponType, weapon); }

        BaseWeapon ReturnSameTypeWeapon(BaseWeapon.Type type) { return _weaponsContainer[type]; }

        bool HaveSameTypeWeapon(BaseWeapon.Type type) { return _weaponsContainer.ContainsKey(type); }

        ///
        public void OnHandleEquip(BaseWeapon.Type type) => _weaponFSM.OnHandleEquip(type);
        public void OnHandleEventStart(BaseWeapon.EventType type) => _weaponFSM.OnHandleEventStart(type);
        public void OnHandleEventEnd() => _weaponFSM.OnHandleEventEnd();

        public void OnHandleReload() => _weaponFSM.OnHandleReload();
        public void OnHandleDrop() => _weaponFSM.OnHandleDrop();

        void InitializeFSM()
        {
            _weaponFSM = new StateMachine<State>();
            Dictionary<State, BaseState> weaponStates = new Dictionary<State, BaseState>();

            BaseState idle = new IdleState(SetState, SetState, SwitchToNewWeapon, ReturnEquipedWeapon);

            BaseState equip = new EquipState(SetState, SwitchToNewWeapon, ResetEquipedWeapon, ReturnSameTypeWeapon, OnWeaponChangeRequested, ReturnEquipedWeapon);

            BaseState reload = new ReloadState(_isTPS, SetState, SetState, SwitchToNewWeapon, ReturnEquipedWeapon);

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
            _weaponFSM.SetState(State.Idle);//, "EquipKnifeFirst", BaseWeapon.Type.Melee); // 초기 State 설정
        }

        public void OnUpdate()
        {
            _weaponFSM.OnUpdate();
            foreach (var weapon in _weaponsContainer) weapon.Value.OnUpdate(); // 무기 루틴 돌려주기
        }
    }
}