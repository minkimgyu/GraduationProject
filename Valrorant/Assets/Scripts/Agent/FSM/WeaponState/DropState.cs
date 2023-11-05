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
        //_storedPlayer.WeaponHolder.NowEquipedWeapon.OnDrop();
        _storedWeaponController.WeaponFSM.RevertToPreviousState();
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

    public void OnStateUpdate()
    {
    }
}
