using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using Agent.Controller;
using System;

namespace Agent.States
{
    public class RightActionState : State
    {
        Action<WeaponController.State> SetState;
        Func<BaseWeapon> ReturnEquipedWeapon;

        public RightActionState(Action<WeaponController.State> SetState, Func<BaseWeapon> ReturnEquipedWeapon)
        {
            this.SetState = SetState;
            this.ReturnEquipedWeapon = ReturnEquipedWeapon;
        }

        public override void OnStateEnter()
        {
            BaseWeapon equipedWeapon = ReturnEquipedWeapon();
            equipedWeapon.OnRightClickStart();
        }

        public override void OnStateExit()
        {
            BaseWeapon equipedWeapon = ReturnEquipedWeapon();
            equipedWeapon.OnRightClickEnd();
        }

        public override void CheckStateChange()
        {
            BaseWeapon equipedWeapon = ReturnEquipedWeapon();

            if (equipedWeapon.CanAutoReload()) SetState?.Invoke(WeaponController.State.Reload);
            if (Input.GetMouseButtonUp(0)) SetState?.Invoke(WeaponController.State.Idle);
        }

        public override void OnStateUpdate()
        {
            BaseWeapon equipedWeapon = ReturnEquipedWeapon();
            equipedWeapon.OnRightClickProgress();
        }
    }
}