using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;

public class EquipState : State
{
    WeaponController _sWC;
    Timer _timer;

    BaseWeapon.Type _weaponType;

    public EquipState(WeaponController weaponController)
    {
        _sWC = weaponController;
        _timer = new Timer();
    }

    public override void OnMessageReceived(BaseWeapon.Type weaponType)
    {
        _weaponType = weaponType;
    }

    public override void CheckStateChange()
    {
        if (_timer.IsFinish) _sWC.WeaponFSM.SetState(WeaponController.WeaponState.Idle);

        _sWC.CheckChangeStateForRooting();
    }

    public override void OnStateExit()
    {
        _timer.Reset();
    }

    public override void OnStateEnter()
    {
        if (_sWC.NowEquipedWeapon != null && _weaponType == _sWC.NowEquipedWeapon.WeaponType)
        {
            _sWC.WeaponFSM.RevertToPreviousState();
            return;
        }

        _sWC.ChangeWeapon(_weaponType);
        _timer.Start(_sWC.NowEquipedWeapon.EquipFinishTime); // µÙ∑π¿Ã
        _weaponType = BaseWeapon.Type.None;
    }

    public override void OnStateUpdate()
    {
        _timer.Update();
    }
}
