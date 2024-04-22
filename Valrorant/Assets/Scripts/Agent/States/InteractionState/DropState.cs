using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using Agent.Controller;
using System;

namespace Agent.States
{
    public class DropState : State
    {
        float _weaponThrowPower;

        Action<WeaponController.State, string, BaseWeapon> SetStateAndSendWeapon;
        Action<WeaponController.State, string, BaseWeapon.Type> SetStateAndSendType;

        Action<BaseWeapon.Type> RemoveWeaponInContainer;
       
        Action RevertToPreviousState;

        Func<BaseWeapon> ReturnEquipedWeapon;
        Func<BaseWeapon.Type, bool> HaveSameTypeWeapon;
        Func<BaseWeapon.Type, BaseWeapon> ReturnSameTypeWeapon;

        Func<WeaponEventBlackboard> ReturnEventBlackboard;
        BaseWeapon _newWeapon;

        public DropState(
            float weaponThrowPower, 
            Action RevertToPreviousState, 
            Action<BaseWeapon.Type> RemoveWeaponInContainer,


            Action<WeaponController.State, string, BaseWeapon> SetStateAndSendWeapon,
            Action<WeaponController.State, string, BaseWeapon.Type> SetStateAndSendType,

            Func<BaseWeapon> ReturnEquipedWeapon, 
            Func<BaseWeapon.Type, bool> HaveSameTypeWeapon, 
            Func<BaseWeapon.Type, BaseWeapon> ReturnSameTypeWeapon, 
            Func<WeaponEventBlackboard> ReturnEventBlackboard)
        {
            _weaponThrowPower = weaponThrowPower;
            this.RevertToPreviousState = RevertToPreviousState;
            this.RemoveWeaponInContainer = RemoveWeaponInContainer;
            this.SetStateAndSendWeapon = SetStateAndSendWeapon;
            this.SetStateAndSendType = SetStateAndSendType;


            this.ReturnEquipedWeapon = ReturnEquipedWeapon;
            this.HaveSameTypeWeapon = HaveSameTypeWeapon;
            this.ReturnSameTypeWeapon = ReturnSameTypeWeapon;

            this.ReturnEventBlackboard = ReturnEventBlackboard;
        }

        public override void OnMessageReceived(string message, BaseWeapon newWeapon)
        {
            Debug.Log(message);
            _newWeapon = newWeapon;
        }

        bool DropWeapon(BaseWeapon weapon, bool activateWeapon = false)
        {
            weapon.ThrowWeapon(_weaponThrowPower);

            if (activateWeapon == true) weapon.gameObject.SetActive(true);

            WeaponEventBlackboard eventBlackboard = ReturnEventBlackboard();
            weapon.OnDrop(eventBlackboard);

            RemoveWeaponInContainer?.Invoke(weapon.WeaponType);
            return true;
        }

        public override void OnStateExit()
        {
            _newWeapon = null; // 초기화해준다.
        }

        public override void OnStateEnter()
        {
            // bool을 리턴해서 만약 false면 Idle로 돌아감

            BaseWeapon equipedWeapon = ReturnEquipedWeapon();
            if (_newWeapon != null)
            {
                if (_newWeapon.WeaponType == equipedWeapon.WeaponType) // 현재 장착하고 있는 무기의 타입과 같은 경우
                {
                    bool canDrop = equipedWeapon.CanDrop();

                    if(canDrop)
                    {
                        DropWeapon(equipedWeapon);
                        SetStateAndSendWeapon?.Invoke(WeaponController.State.Root, "RootWeapon", _newWeapon);
                    }
                    else RevertToPreviousState?.Invoke();
                }
                else
                {
                    BaseWeapon sameTypeWeapon = ReturnSameTypeWeapon(_newWeapon.WeaponType);
                    if(sameTypeWeapon == null)
                    {
                        SetStateAndSendWeapon?.Invoke(WeaponController.State.Root, "RootWeaponWithNoDrop", _newWeapon);
                        return;
                    }

                    bool canDrop = sameTypeWeapon.CanDrop();
                    if(canDrop)
                    {
                        DropWeapon(sameTypeWeapon);
                        SetStateAndSendWeapon?.Invoke(WeaponController.State.Root, "RootWeapon", _newWeapon);
                    }
                    else RevertToPreviousState?.Invoke();
                }
            }
            else
            {
                BaseWeapon.Type type = ReturnNextWeaponType(equipedWeapon.WeaponType);
                bool canDrop = equipedWeapon.CanDrop();
                if (canDrop)
                {
                    DropWeapon(equipedWeapon);
                    SetStateAndSendType?.Invoke(WeaponController.State.Equip, "EquipNextWeapon", type);
                }
                else RevertToPreviousState?.Invoke();
            }
        }

        public BaseWeapon.Type ReturnNextWeaponType(BaseWeapon.Type currentType)
        {
            if (currentType == BaseWeapon.Type.Main)
            {
                if (HaveSameTypeWeapon(currentType)) return BaseWeapon.Type.Sub;
                else return BaseWeapon.Type.Melee;
            }
            else return BaseWeapon.Type.Melee;
        }
    }
}