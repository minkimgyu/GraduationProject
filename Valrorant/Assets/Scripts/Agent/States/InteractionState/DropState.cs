using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using Agent.Controller;

namespace Agent.States
{
    public class DropState : State
    {
        WeaponController _sWC;
        bool _goToRootState;

        public DropState(WeaponController _weaponController)
        {
            _sWC = _weaponController;
        }

        public override void OnMessageReceived(bool goToRootState)
        {
            _goToRootState = goToRootState;
        }

        public override void OnStateEnter()
        {
            // bool�� �����ؼ� ���� false�� Idle�� ���ư�

            if (_goToRootState)
            {
                if (_sWC.IsInteractingAndEquipedWeaponSameType())
                {
                    bool canDrop = _sWC.DropNowEquipedWeapon();
                    if (canDrop == false) _sWC.WeaponFSM.RevertToPreviousState();
                    else _sWC.WeaponFSM.SetState(WeaponController.WeaponState.Root, true); // ���� ������ ���⸦ ������ ���ο� ���⸦ ����
                }
                else
                {
                    // �ش� ���Ⱑ ���� ���� �н��ϰ� �ٷ� Root�� ����
                    bool canDrop = _sWC.DropWeaponSameTypeWithNowInteracting();
                    if (canDrop == false) _sWC.WeaponFSM.RevertToPreviousState();
                    else _sWC.WeaponFSM.SetState(WeaponController.WeaponState.Root, false); // ���� ������ ����� �ٸ� Ÿ���� ���⸦ ����
                }

                _goToRootState = false;
            }
            else
            {
                BaseWeapon.Type type = _sWC.ReturnNextEquipWeaponTypeWhenDrop();

                bool canDrop = _sWC.DropNowEquipedWeapon();
                if (canDrop == false) _sWC.WeaponFSM.RevertToPreviousState();
                else _sWC.WeaponFSM.SetState(WeaponController.WeaponState.Equip, type); // �ٸ� ����� ���������
            }
        }
    }
}