using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class DropState : IState
{
    WeaponController _storedWeaponController;

    public DropState(WeaponController player)
    {
        _storedWeaponController = player;
    }

    public void CheckStateChange()
    {
    }

    public void OnStateCollisionEnter(Collision collision)
    {
    }

    public void OnStateEnter()
    {
        if (_storedWeaponController.NowEquipedWeapon.CanDrop())
        {
            _storedWeaponController.DropWeapon();
            // 두번째 무기를 넣어준다.

            _storedWeaponController.ResetWeaponTypeWhenDrop();
            _storedWeaponController.WeaponFSM.SetState(WeaponController.WeaponState.Equip);
        }
        else
        {
            _storedWeaponController.WeaponFSM.RevertToPreviousState();
        }
    }

    public void OnStateExit()
    {
    }

    public void OnStateFixedUpdate()
    {
    }

    public void OnStateLateUpdate()
    {
    }

    public void OnStateTriggerEnter(Collider collider) { }

    public void OnStateTriggerExit(Collider collider) { }

    public void OnStateUpdate()
    {
    }
}
