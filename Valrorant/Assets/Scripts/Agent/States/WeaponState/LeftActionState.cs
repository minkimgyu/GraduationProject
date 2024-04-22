using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using Agent.Controller;
using System;

namespace Agent.States
{
    public class LeftActionState : State
    {
        Action<WeaponController.State> SetState;
        Func<BaseWeapon> ReturnEquipedWeapon;

        public LeftActionState(Action<WeaponController.State> SetState, Func<BaseWeapon> ReturnEquipedWeapon)
        {
            this.SetState = SetState;
            this.ReturnEquipedWeapon = ReturnEquipedWeapon;
        }

        public override void OnStateEnter()
        {
            BaseWeapon equipedWeapon = ReturnEquipedWeapon();
            equipedWeapon.OnLeftClickStart();
        }

        public override void OnStateExit()
        {
            BaseWeapon equipedWeapon = ReturnEquipedWeapon();
            equipedWeapon.OnLeftClickEnd();
        }

        public override void CheckStateChange()
        {
            // 사격 도중 총알이 떨어진 경우, State에 들어왔을 때는 총알이 존재했지만
            // Update 중 총알이 다 떨어진 경우

            BaseWeapon equipedWeapon = ReturnEquipedWeapon();

            if (equipedWeapon.CanAutoReload()) SetState?.Invoke(WeaponController.State.Reload);
            if (Input.GetMouseButtonUp(0)) SetState?.Invoke(WeaponController.State.Idle);
        }

        public override void OnStateUpdate()
        {
            BaseWeapon equipedWeapon = ReturnEquipedWeapon();
            equipedWeapon.OnLeftClickProcess();
        }
    }
}