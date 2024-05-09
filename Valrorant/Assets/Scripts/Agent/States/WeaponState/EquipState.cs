using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;
using Agent.Controller;

namespace Agent.States
{
    public class EquipState : State
    {
        StopwatchTimer _timer;
        BaseWeapon.Type _weaponTypeToEquip;

        Action<WeaponController.State> SetState;
        Action<BaseWeapon> SwitchToNewWeapon;

        Action<BaseWeapon> ResetEquipedWeapon;

        Action<float> OnWeaponChangeRequested;

        Func<BaseWeapon.Type, BaseWeapon> ReturnSameTypeWeapon;

        Func<BaseWeapon> ReturnEquipedWeapon;


        public EquipState(
            Action<WeaponController.State> SetState,
            Action<BaseWeapon> SwitchToNewWeapon,
            Action<BaseWeapon> ResetEquipedWeapon,

            Func<BaseWeapon.Type, BaseWeapon> ReturnSameTypeWeapon,

            Action<float> OnWeaponChangeRequested,

            Func<BaseWeapon> ReturnEquipedWeapon)
        {
            _timer = new StopwatchTimer();

            this.SetState = SetState;
            this.SwitchToNewWeapon = SwitchToNewWeapon;
            this.ResetEquipedWeapon = ResetEquipedWeapon;

            this.ReturnSameTypeWeapon = ReturnSameTypeWeapon;

            this.OnWeaponChangeRequested = OnWeaponChangeRequested;

            this.ReturnEquipedWeapon = ReturnEquipedWeapon;
        }

        public override void OnMessageReceived(string message, BaseWeapon.Type weaponTypeToEquip)
        {
            Debug.Log(message);
            _weaponTypeToEquip = weaponTypeToEquip;
        }

        public override void CheckStateChange()
        {
            if (_timer.CurrentState == StopwatchTimer.State.Finish)
            {
                SetState.Invoke(WeaponController.State.Idle);
                return;
            }
        }

        /// <summary>
        /// �̰Ŵ� ���� �̺�Ʈ�� WeaponController���� �ۼ�����
        /// </summary>
        public override void OnWeaponReceived(BaseWeapon weapon)
        {
            SwitchToNewWeapon?.Invoke(weapon);
        }

        public void EquipWeapon(BaseWeapon.Type weaponType)
        {
            BaseWeapon equipedWeapon = ReturnEquipedWeapon();

            if(equipedWeapon != null)
            {
                equipedWeapon.OnUnEquip();
                equipedWeapon.gameObject.SetActive(false);
            }

            BaseWeapon weaponToEquip = ReturnSameTypeWeapon(weaponType);
            ResetEquipedWeapon?.Invoke(weaponToEquip);

            weaponToEquip.gameObject.SetActive(true);
            weaponToEquip.OnEquip();

            OnWeaponChangeRequested?.Invoke(weaponToEquip.SlowDownRatioByWeaponWeight);
        }

        public override void OnStateEnter()
        {
            EquipWeapon(_weaponTypeToEquip);

            BaseWeapon weapon = ReturnEquipedWeapon();
            _timer.Start(weapon.EquipFinishTime); // ������
        }

        public override void OnStateExit() => _timer.Reset();
    }
}