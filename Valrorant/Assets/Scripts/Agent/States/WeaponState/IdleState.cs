using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using Agent.Controller;
using System;

namespace Agent.States
{
    public class IdleState : State
    {
        Action<WeaponController.State> SetState;
        Action<WeaponController.State, string, BaseWeapon.Type> SetStateUsingWeaponType;

        Action<BaseWeapon> SwitchToNewWeapon;
        Func<BaseWeapon> ReturnEquipedWeapon;

        public IdleState(
            Action<WeaponController.State> SetState,
            Action<WeaponController.State, string, BaseWeapon.Type> SetStateUsingWeaponType,
            Action<BaseWeapon> SwitchToNewWeapon,

            Func<BaseWeapon> ReturnEquipedWeapon)
        {
            this.SetState = SetState;
            this.SetStateUsingWeaponType = SetStateUsingWeaponType;
            this.SwitchToNewWeapon = SwitchToNewWeapon;

            this.ReturnEquipedWeapon = ReturnEquipedWeapon;
        }

        public override void OnStateEnter()
        {
            BaseWeapon equipedWeapon = ReturnEquipedWeapon();
            if (equipedWeapon == null) return;

            if (equipedWeapon.CanAutoReload() == false) return;

            SetState.Invoke(WeaponController.State.Reload);
        }

        public override void OnHandleEquip(BaseWeapon.Type type)
        {
            BaseWeapon equipedWeapon = ReturnEquipedWeapon();
            if (equipedWeapon != null && equipedWeapon.WeaponType == type) return; // 이미 같은 타입의 아이템이 장착되어 있으면 리턴
            SetStateUsingWeaponType?.Invoke(WeaponController.State.Equip, "SendWeaponTypeToEquip", type);
        }

        public override void OnHandleEventStart(BaseWeapon.EventType type)
        {
            switch (type)
            {
                case BaseWeapon.EventType.Main:
                    SetState?.Invoke(WeaponController.State.LeftAction);
                    break;
                case BaseWeapon.EventType.Sub:
                    SetState?.Invoke(WeaponController.State.RightAction);
                    break;
            }
        }

        public override void OnHandleDrop()
        {
            SetState?.Invoke(WeaponController.State.Drop);
        }

        public override void OnHandleReload()
        {
            BaseWeapon equipedWeapon = ReturnEquipedWeapon();
            if (equipedWeapon.CanReload() == false) return;

            SetState?.Invoke(WeaponController.State.Reload);
        }

        public override void OnWeaponReceived(BaseWeapon weapon)
        {
            SwitchToNewWeapon?.Invoke(weapon);
        }
    }
}