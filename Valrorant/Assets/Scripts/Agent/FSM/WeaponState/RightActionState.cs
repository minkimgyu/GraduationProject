using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class RightActionState : IState
{
    WeaponController _storedWeaponController;

    public RightActionState(WeaponController player)
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
        _storedWeaponController.NowEquipedWeapon.OnRightClickStart();
    }

    public void OnStateExit()
    {
        _storedWeaponController.NowEquipedWeapon.OnRightClickEnd();
    }

    public void OnStateFixedUpdate()
    {
    }

    public void OnStateLateUpdate()
    {
    }

    public void OnStateUpdate()
    {
        _storedWeaponController.NowEquipedWeapon.OnRightClickProgress();

        if (_storedWeaponController.NowEquipedWeapon.CanAutoReload())
        {
            _storedWeaponController.WeaponFSM.SetState(WeaponController.WeaponState.Reload);
        }

        if (Input.GetMouseButtonUp(1))
        {
            _storedWeaponController.WeaponFSM.RevertToPreviousState();
        }
    }
}
