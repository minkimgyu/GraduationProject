using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class IdleState : IState
{
    WeaponController _storedWeaponController;

    public IdleState(WeaponController weaponController)
    {
        _storedWeaponController = weaponController;
    }

    void GoToEquipStateUsingType(BaseWeapon.Type weaponType)
    {
        bool nowContain = _storedWeaponController.CanChangeWeapon(weaponType);
        if (nowContain == false) return;

        _storedWeaponController.NowEquipedweaponType = weaponType;
        _storedWeaponController.WeaponFSM.SetState(WeaponController.WeaponState.Equip);
    }

    public void CheckStateChange()
    {
        // 이부분은 타입을 보고 장착하도록 변경해준다.
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GoToEquipStateUsingType(BaseWeapon.Type.Main);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GoToEquipStateUsingType(BaseWeapon.Type.Sub);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GoToEquipStateUsingType(BaseWeapon.Type.Melee);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            _storedWeaponController.WeaponFSM.SetState(WeaponController.WeaponState.Drop);
        }

        if (Input.GetMouseButtonDown(0))
        {
            _storedWeaponController.WeaponFSM.SetState(WeaponController.WeaponState.LeftAction);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            _storedWeaponController.WeaponFSM.SetState(WeaponController.WeaponState.RightAction);
        }

        if (Input.GetKeyDown(KeyCode.R) && _storedWeaponController.NowEquipedWeapon.CanReload())
        {
            _storedWeaponController.WeaponFSM.SetState(WeaponController.WeaponState.Reload);
        }
    }

    public void OnStateCollisionEnter(Collision collision)
    {
    }

    public void OnStateEnter()
    {
        if(_storedWeaponController.NowEquipedWeapon.CanAutoReload())
        {
            _storedWeaponController.WeaponFSM.SetState(WeaponController.WeaponState.Reload);
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
