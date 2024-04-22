using Agent.Controller;
using FSM;
using UnityEngine;
using System;

namespace Agent.States
{
    public class RootState : State
    {
        BaseWeapon _newWeapon;

        Action<WeaponController.State> SetState;
        Action<WeaponController.State, string, BaseWeapon.Type> SetStateAndSendType;
        Action<BaseWeapon> AddWeaponToContainer;

        Func<BaseWeapon> ReturnEquipedWeapon;
        Func<Transform> ReturnWeaponParent;
        Func<WeaponEventBlackboard> ReturnEventBlackboard;

        public RootState(
            Action<WeaponController.State> SetState,
            Action<WeaponController.State, string, BaseWeapon.Type> SetStateAndSendType,
            Action<BaseWeapon> AddWeaponToContainer,

            Func<BaseWeapon> ReturnEquipedWeapon,
            Func<Transform> ReturnWeaponParent,
            Func<WeaponEventBlackboard> ReturnEventBlackboard
            )
        {
            this.SetState = SetState;
            this.SetStateAndSendType = SetStateAndSendType;
            this.AddWeaponToContainer = AddWeaponToContainer;

            this.ReturnEquipedWeapon = ReturnEquipedWeapon;
            this.ReturnWeaponParent = ReturnWeaponParent;
            this.ReturnEventBlackboard = ReturnEventBlackboard;
        }

        public override void OnMessageReceived(string message, BaseWeapon newWeapon)
        {
            Debug.Log(message);
            _newWeapon = newWeapon;
        }

        void AttachWeaponToArm(BaseWeapon weapon)
        {
            Transform weaponParent = ReturnWeaponParent();
            weapon.transform.SetParent(weaponParent);
            weapon.PositionWeapon(false);
        }

        public void RootWeapon()
        {
            WeaponEventBlackboard blackboard = ReturnEventBlackboard();
            _newWeapon.OnRooting(blackboard);

            AddWeaponToContainer?.Invoke(_newWeapon);
            AttachWeaponToArm(_newWeapon);
        }

        public override void OnStateEnter()
        {
            // 현재 장착한 무기랑 같은 타입의 무기인 경우
            // 아니면 다른 경우
            RootWeapon();
            BaseWeapon equipedWeapon = ReturnEquipedWeapon();

            if (_newWeapon != null && equipedWeapon.WeaponType == _newWeapon.WeaponType)
            {
                SetStateAndSendType?.Invoke(WeaponController.State.Equip, "EquipWeapon", _newWeapon.WeaponType);
            }
            else SetState?.Invoke(WeaponController.State.Idle);
        }

        public override void OnStateExit()
        {
            _newWeapon = null;
        }
    }
}