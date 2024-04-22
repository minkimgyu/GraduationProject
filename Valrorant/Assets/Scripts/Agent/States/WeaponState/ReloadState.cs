using FSM;
using UnityEngine;
using Agent.Controller;
using System;

namespace Agent.States
{
    public class ReloadState : State
    {
        Action<WeaponController.State> SetState;
        Func<BaseWeapon> ReturnEquipedWeapon;

        Action<BaseWeapon.Type> GoToEquipState;
        Action<BaseWeapon> SwitchToNewWeapon;

        public ReloadState(
            Action<WeaponController.State> SetState,
            Action<BaseWeapon.Type> GoToEquipState,
            Action<BaseWeapon> SwitchToNewWeapon,

            Func<BaseWeapon> ReturnEquipedWeapon
             )
        {
            this.SetState = SetState;
            this.GoToEquipState = GoToEquipState;
            this.SwitchToNewWeapon = SwitchToNewWeapon;

            this.ReturnEquipedWeapon = ReturnEquipedWeapon;
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

            BaseWeapon equipedWeapon = ReturnEquipedWeapon();

            bool nowCancelMainAction = equipedWeapon.CanCancelReloadAndGoToMainAction();
            if (nowCancelMainAction) SetState?.Invoke(WeaponController.State.LeftAction);

            bool nowCancelSubAction = equipedWeapon.CanCancelReloadAndGoToSubAction();
            if (nowCancelSubAction) SetState?.Invoke(WeaponController.State.RightAction);

            bool isFinish = equipedWeapon.IsReloadFinish();
            if (isFinish) SetState?.Invoke(WeaponController.State.Idle);
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
            equipedWeapon.OnReload();
        }
    }
}