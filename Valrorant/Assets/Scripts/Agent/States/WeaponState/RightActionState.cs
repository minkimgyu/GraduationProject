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

            if (equipedWeapon.CanAutoReload())
            {
                SetState?.Invoke(WeaponController.State.Reload);
                return;
            }
        }

        public override void OnHandleEventEnd()
        {
            SetState?.Invoke(WeaponController.State.Idle);
        }

        public override void OnHandleReload()
        {
            BaseWeapon equipedWeapon = ReturnEquipedWeapon();
            if (equipedWeapon.CanReload() == false) return;

            SetState?.Invoke(WeaponController.State.Reload);
        }

        public override void OnStateUpdate()
        {
            BaseWeapon equipedWeapon = ReturnEquipedWeapon();
            equipedWeapon.OnRightClickProgress();
        }
    }
}