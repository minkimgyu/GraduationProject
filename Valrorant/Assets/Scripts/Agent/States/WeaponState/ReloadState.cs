using FSM;
using UnityEngine;
using Agent.Controller;

namespace Agent.States
{
    public class ReloadState : State
    {
        WeaponController _sWC;

        public ReloadState(WeaponController player)
        {
            _sWC = player;
        }

        public override void CheckStateChange()
        {
            _sWC.GetWeaponChangeInput();

            bool nowCancelMainAction = _sWC.NowEquipedWeapon.CancelReloadAndGoToMainAction();
            if (nowCancelMainAction)
            {
                _sWC.WeaponFSM.SetState(WeaponController.WeaponState.LeftAction);
            }

            bool nowCancelSubAction = _sWC.NowEquipedWeapon.CancelReloadAndGoToSubAction();
            if (nowCancelSubAction)
            {
                _sWC.WeaponFSM.SetState(WeaponController.WeaponState.RightAction);
            }

            bool canExit = _sWC.NowEquipedWeapon.IsReloadFinish();
            if (canExit)
            {
                _sWC.WeaponFSM.SetState(WeaponController.WeaponState.Idle);
            }

            _sWC.CheckChangeStateForRooting();
        }

        public override void OnStateExit()
        {
            if (_sWC.NowEquipedWeapon != null) _sWC.NowEquipedWeapon.ResetReload();
        }

        public override void OnStateEnter()
        {
            _sWC.NowEquipedWeapon.OnReload();
        }
    }
}