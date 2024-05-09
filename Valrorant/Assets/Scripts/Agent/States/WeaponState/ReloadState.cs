using FSM;
using UnityEngine;
using Agent.Controller;
using System;

namespace Agent.States
{
    public class ReloadState : State
    {
        Action<WeaponController.State> SetState;
        Action<WeaponController.State, string, BaseWeapon.Type> SetStateUsingWeaponType;

        Func<BaseWeapon> ReturnEquipedWeapon;
        Action<BaseWeapon> SwitchToNewWeapon;
        bool _isTPS;

        public ReloadState(
            bool isTPS,
            Action<WeaponController.State> SetState,
            Action<WeaponController.State, string, BaseWeapon.Type> SetStateUsingWeaponType,
            Action<BaseWeapon> SwitchToNewWeapon,

            Func<BaseWeapon> ReturnEquipedWeapon
             )
        {
            _isTPS = isTPS;

            this.SetState = SetState;
            this.SetStateUsingWeaponType = SetStateUsingWeaponType;
            this.SwitchToNewWeapon = SwitchToNewWeapon;

            this.ReturnEquipedWeapon = ReturnEquipedWeapon;
        }

        public override void CheckStateChange()
        {
            BaseWeapon equipedWeapon = ReturnEquipedWeapon();
            bool isFinish = equipedWeapon.IsReloadFinish();
            if (isFinish)
            {
                SetState?.Invoke(WeaponController.State.Idle);
                return;
            }
        }

        public override void OnHandleEquip(BaseWeapon.Type type)
        {
            BaseWeapon equipedWeapon = ReturnEquipedWeapon();
            if (equipedWeapon.WeaponType == type) return; // 이미 같은 타입의 아이템이 장착되어 있으면 리턴
            SetStateUsingWeaponType?.Invoke(WeaponController.State.Equip, "SendWeaponTypeToEquip", type);
        }

        public override void OnHandleEventStart(BaseWeapon.EventType type)
        {
            BaseWeapon equipedWeapon = ReturnEquipedWeapon();

            switch (type)
            {
                case BaseWeapon.EventType.Main:
                    bool nowCancelMainAction = equipedWeapon.CanCancelReloadAndGoToMainAction();
                    if (nowCancelMainAction == false) break;

                    SetState?.Invoke(WeaponController.State.LeftAction);
                    break;
                case BaseWeapon.EventType.Sub:
                    bool nowCancelSubAction = equipedWeapon.CanCancelReloadAndGoToSubAction();
                    if (nowCancelSubAction == false) break;

                    SetState?.Invoke(WeaponController.State.RightAction);
                    break;
            }
        }

        public override void OnWeaponReceived(BaseWeapon weapon)
        {
            SwitchToNewWeapon?.Invoke(weapon);
        }

        public override void OnStateExit()
        {
            BaseWeapon equipedWeapon = ReturnEquipedWeapon();
            equipedWeapon.ResetReload();
        }

        public override void OnStateEnter()
        {
            BaseWeapon equipedWeapon = ReturnEquipedWeapon();
            equipedWeapon.OnReload(_isTPS);
        }
    }
}