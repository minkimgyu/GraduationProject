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

        Action<float> OnWeaponWeightChangeRequested;
        Action<BaseWeapon.Name> OnProfileChangeRequested;

        Func<BaseWeapon.Type, BaseWeapon> ReturnSameTypeWeapon;

        Func<BaseWeapon> ReturnEquipedWeapon;


        public EquipState(
            Action<WeaponController.State> SetState,
            Action<BaseWeapon> SwitchToNewWeapon,
            Action<BaseWeapon> ResetEquipedWeapon,

            Func<BaseWeapon.Type, BaseWeapon> ReturnSameTypeWeapon,

            Action<float> OnWeaponWeightChangeRequested,
            Action<BaseWeapon.Name> OnProfileChangeRequested,

            Func<BaseWeapon> ReturnEquipedWeapon)
        {
            _timer = new StopwatchTimer();

            this.SetState = SetState;
            this.SwitchToNewWeapon = SwitchToNewWeapon;
            this.ResetEquipedWeapon = ResetEquipedWeapon;

            this.ReturnSameTypeWeapon = ReturnSameTypeWeapon;

            this.OnProfileChangeRequested = OnProfileChangeRequested;
            this.OnWeaponWeightChangeRequested = OnWeaponWeightChangeRequested;

            this.ReturnEquipedWeapon = ReturnEquipedWeapon;
        }

        public override void OnMessageReceived(string message, BaseWeapon.Type weaponTypeToEquip)
        {
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
        /// 이거는 공통 이벤트로 WeaponController에서 작성하자
        /// </summary>
        public override void OnWeaponReceived(BaseWeapon weapon)
        {
            SwitchToNewWeapon?.Invoke(weapon);
        }

        public void EquipWeapon(BaseWeapon weaponToEquip)
        {
            BaseWeapon equipedWeapon = ReturnEquipedWeapon();

            if(equipedWeapon != null)
            {
                equipedWeapon.OnUnEquip();
                equipedWeapon.gameObject.SetActive(false);
            }

            ResetEquipedWeapon?.Invoke(weaponToEquip);

            weaponToEquip.gameObject.SetActive(true);
            weaponToEquip.OnEquip();

            OnProfileChangeRequested?.Invoke(weaponToEquip.WeaponName);
            OnWeaponWeightChangeRequested?.Invoke(weaponToEquip.SlowDownRatioByWeaponWeight);
        }

        public override void OnStateEnter()
        {
            BaseWeapon weaponToEquip = ReturnSameTypeWeapon(_weaponTypeToEquip);
            if (weaponToEquip == null)
            {
                SetState(WeaponController.State.Idle);
                return;
            }

            EquipWeapon(weaponToEquip);

            BaseWeapon weapon = ReturnEquipedWeapon();
            _timer.Start(weapon.EquipFinishTime); // 딜레이
        }

        public override void OnStateExit() => _timer.Reset();
    }
}