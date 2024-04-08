using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using Agent.Controller;

namespace Agent.States
{
    public class RightActionState : State
    {
        WeaponController _sWC;

        public RightActionState(WeaponController player)
        {
            _sWC = player;
        }

        public override void OnStateEnter()
        {
            _sWC.NowEquipedWeapon.OnRightClickStart();
        }

        public override void OnStateExit()
        {
            _sWC.NowEquipedWeapon.OnRightClickEnd();
        }

        public override void CheckStateChange()
        {
            if (_sWC.NowEquipedWeapon.CanAutoReload())
            {
                _sWC.WeaponFSM.SetState(WeaponController.WeaponState.Reload);
            }

            if (Input.GetMouseButtonUp(1))
            {
                _sWC.WeaponFSM.SetState(WeaponController.WeaponState.Idle);
            }

            _sWC.CheckChangeStateForRooting();
        }

        public override void OnStateUpdate()
        {
            _sWC.NowEquipedWeapon.OnRightClickProgress();
        }
    }
}