using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using Agent.Controller;

namespace Agent.States
{
    public class RootState : State
    {
        WeaponController _sWC;

        bool _goToEquipState;

        public RootState(WeaponController weaponController)
        {
            _sWC = weaponController;
        }

        public override void OnMessageReceived(bool goToEquipState)
        {
            _goToEquipState = goToEquipState;
        }

        public override void OnStateEnter()
        {
            // ���� ������ ����� ���� Ÿ���� ������ ���
            // �ƴϸ� �ٸ� ���

            BaseWeapon.Type tmpType = _sWC.RootWeaponAndReturnType();

            if (_goToEquipState)
            {
                _sWC.StoredWeaponWhenInteracting = null;
                _sWC.WeaponFSM.SetState(WeaponController.WeaponState.Equip, tmpType);
            }
            else
            {
                _sWC.StoredWeaponWhenInteracting = null;
                _sWC.WeaponFSM.SetState(WeaponController.WeaponState.Idle);
            }
        }
    }
}