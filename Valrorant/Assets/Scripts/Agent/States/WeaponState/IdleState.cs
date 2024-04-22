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

        Action<BaseWeapon.Type> GoToEquipState;
        Action<BaseWeapon> SwitchToNewWeapon;

        Func<BaseWeapon> ReturnEquipedWeapon;


        public IdleState(
            Action<WeaponController.State> SetState,
            Action<BaseWeapon.Type> GoToEquipState,
            Action<BaseWeapon> SwitchToNewWeapon,

            Func<BaseWeapon> ReturnEquipedWeapon)
        {
            this.SetState = SetState;
            this.GoToEquipState = GoToEquipState;
            this.SwitchToNewWeapon = SwitchToNewWeapon;

            this.ReturnEquipedWeapon = ReturnEquipedWeapon;
        }

        public override void OnStateEnter()
        {
            BaseWeapon equipedWeapon = ReturnEquipedWeapon();
            if (equipedWeapon.CanAutoReload() == false) return;

            SetState.Invoke(WeaponController.State.Reload);
        }

        public override void CheckStateChange()
        {
            // 이부분은 타입을 보고 장착하도록 변경해준다.
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                GoToEquipState?.Invoke(BaseWeapon.Type.Main);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                GoToEquipState?.Invoke(BaseWeapon.Type.Sub);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                GoToEquipState?.Invoke(BaseWeapon.Type.Melee);
            }


            if (Input.GetMouseButtonDown(0))
            {
                SetState?.Invoke(WeaponController.State.LeftAction);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                SetState?.Invoke(WeaponController.State.RightAction);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                BaseWeapon equipedWeapon = ReturnEquipedWeapon();
                if (equipedWeapon.CanReload() == false) return;

                SetState?.Invoke(WeaponController.State.Reload);
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                SetState?.Invoke(WeaponController.State.Drop);
            }
        }

        public override void OnWeaponReceived(BaseWeapon weapon)
        {
            SwitchToNewWeapon?.Invoke(weapon);
        }
    }
}