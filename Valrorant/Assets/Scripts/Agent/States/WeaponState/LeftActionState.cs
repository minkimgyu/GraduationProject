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
            // ��� ���� �Ѿ��� ������ ���, State�� ������ ���� �Ѿ��� ����������
            // Update �� �Ѿ��� �� ������ ���

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