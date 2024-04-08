using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using Agent.Controller;

namespace Agent.States
{
    public class IdleState : State
    {
        WeaponController _sWC;

        public IdleState(WeaponController weaponController)
        {
            _sWC = weaponController;
        }

        public override void CheckStateChange()
        {
            _sWC.GetWeaponChangeInput();

            if (Input.GetMouseButtonDown(0))
            {
                _sWC.WeaponFSM.SetState(WeaponController.WeaponState.LeftAction);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                _sWC.WeaponFSM.SetState(WeaponController.WeaponState.RightAction);
            }

            if (Input.GetKeyDown(KeyCode.R) && _sWC.NowEquipedWeapon.CanReload())
            {
                _sWC.WeaponFSM.SetState(WeaponController.WeaponState.Reload);
            }

            if (Input.GetKeyDown(KeyCode.G) && _sWC.NowEquipedWeapon.CanDrop() == true)
            {
                _sWC.WeaponFSM.SetState(WeaponController.WeaponState.Drop, false);
            }

            _sWC.CheckChangeStateForRooting();
        }

        public override void OnStateEnter()
        {
            if (_sWC.NowEquipedWeapon.CanAutoReload())
            {
                _sWC.WeaponFSM.SetState(WeaponController.WeaponState.Reload);
            }
        }
    }

}