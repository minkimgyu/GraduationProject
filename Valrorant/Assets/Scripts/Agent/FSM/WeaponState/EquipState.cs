using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;

public class EquipState : IState
{
    WeaponController _storedWeaponController;
    Timer _timer;

    public EquipState(WeaponController weaponController)
    {
        _storedWeaponController = weaponController;
        _timer = new Timer();
    }

    public void CheckStateChange()
    {
        if (_timer.IsFinish) _storedWeaponController.WeaponFSM.SetState(WeaponController.WeaponState.Idle);
    }

    public void OnStateCollisionEnter(Collision collision)
    {
    }

    public void OnStateEnter()
    {
        _storedWeaponController.ChangeWeapon();
        _timer.Start(_storedWeaponController.NowEquipedWeapon.EquipFinishTime); // µÙ∑π¿Ã
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
        _timer.Update();
    }
}
