using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using Agent.Controller;

namespace Agent.States
{
    public class LeftActionState : State
    {
        WeaponController _sWC;

        public LeftActionState(WeaponController storedWeaponController)
        {
            _sWC = storedWeaponController;
        }

        public override void OnStateEnter()
        {
            _sWC.NowEquipedWeapon.OnLeftClickStart();
        }

        public override void OnStateExit()
        {
            _sWC.NowEquipedWeapon.OnLeftClickEnd();
        }

        public override void CheckStateChange()
        {
            // ��� ���� �Ѿ��� ������ ���, State�� ������ ���� �Ѿ��� ����������
            // Update �� �Ѿ��� �� ������ ���
            if (_sWC.NowEquipedWeapon.CanAutoReload())
            {
                _sWC.WeaponFSM.SetState(WeaponController.WeaponState.Reload);
            }

            if (Input.GetMouseButtonUp(0))
            {
                _sWC.WeaponFSM.SetState(WeaponController.WeaponState.Idle);
            }

            _sWC.CheckChangeStateForRooting();
        }

        public override void OnStateUpdate()
        {
            _sWC.NowEquipedWeapon.OnLeftClickProgress();
        }
    }
}