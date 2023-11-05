using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class IdleState : IState
{
    WeaponController _weaponController;

    public IdleState(WeaponController weaponController)
    {
        _weaponController = weaponController;
    }

    public void CheckStateChange()
    {

        // 이부분은 타입을 보고 장착하도록 변경해준다.

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _weaponController.WeaponIndex = 0;
            _weaponController.WeaponFSM.SetState(WeaponController.WeaponState.Equip);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _weaponController.WeaponIndex = 1;
            _weaponController.WeaponFSM.SetState(WeaponController.WeaponState.Equip);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _weaponController.WeaponIndex = 2;
            _weaponController.WeaponFSM.SetState(WeaponController.WeaponState.Equip);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            _weaponController.WeaponFSM.SetState(WeaponController.WeaponState.Drop);
        }

        if (Input.GetMouseButtonDown(0))
        {
            _weaponController.WeaponFSM.SetState(WeaponController.WeaponState.LeftAction);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            _weaponController.WeaponFSM.SetState(WeaponController.WeaponState.RightAction);
        }

        if (Input.GetKeyDown(KeyCode.R) && _weaponController.NowEquipedWeapon.CanReload())
        {
            _weaponController.WeaponFSM.SetState(WeaponController.WeaponState.Reload);
        }
    }

    public void OnStateCollisionEnter(Collision collision)
    {
    }

    public void OnStateEnter()
    {
        if(_weaponController.NowEquipedWeapon.CanAutoReload())
        {
            _weaponController.WeaponFSM.SetState(WeaponController.WeaponState.Reload);
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

    public void OnStateUpdate()
    {
    }
}
