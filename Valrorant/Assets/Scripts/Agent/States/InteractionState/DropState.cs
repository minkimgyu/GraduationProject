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
            // bool을 리턴해서 만약 false면 Idle로 돌아감

            if (_goToRootState)
            {
                if (_sWC.IsInteractingAndEquipedWeaponSameType())
                {
                    bool canDrop = _sWC.DropNowEquipedWeapon();
                    if (canDrop == false) _sWC.WeaponFSM.RevertToPreviousState();
                    else _sWC.WeaponFSM.SetState(WeaponController.WeaponState.Root, true); // 현재 장착한 무기를 버리고 새로운 무기를 장착
                }
                else
                {
                    // 해당 무기가 없는 경우는 패스하고 바로 Root로 진행
                    bool canDrop = _sWC.DropWeaponSameTypeWithNowInteracting();
                    if (canDrop == false) _sWC.WeaponFSM.RevertToPreviousState();
                    else _sWC.WeaponFSM.SetState(WeaponController.WeaponState.Root, false); // 현재 장착한 무기와 다른 타입의 무기를 버림
                }

                _goToRootState = false;
            }
            else
            {
                BaseWeapon.Type type = _sWC.ReturnNextEquipWeaponTypeWhenDrop();

                bool canDrop = _sWC.DropNowEquipedWeapon();
                if (canDrop == false) _sWC.WeaponFSM.RevertToPreviousState();
                else _sWC.WeaponFSM.SetState(WeaponController.WeaponState.Equip, type); // 다른 무기로 변경시켜줌
            }
        }
    }
}