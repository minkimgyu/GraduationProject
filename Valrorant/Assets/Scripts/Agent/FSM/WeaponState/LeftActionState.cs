using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class LeftActionState : IState
{
    WeaponController _storedWeaponController;

    public LeftActionState(WeaponController storedWeaponController)
    {
        _storedWeaponController = storedWeaponController;
    }

    public void CheckStateChange()
    {
    }

    public void OnStateCollisionEnter(Collision collision)
    {
    }

    public void OnStateEnter()
    {
        _storedWeaponController.NowEquipedWeapon.OnLeftClickStart();
    }

    public void OnStateExit()
    {
        _storedWeaponController.NowEquipedWeapon.OnLeftClickEnd();
    }

    public void OnStateFixedUpdate()
    {
    }

    public void OnStateLateUpdate()
    {
    }

    public void OnStateUpdate()
    {
        _storedWeaponController.NowEquipedWeapon.OnLeftClickProgress();

        // ��� ���� �Ѿ��� ������ ���, State�� ������ ���� �Ѿ��� ����������
        // Update �� �Ѿ��� �� ������ ���

        if(_storedWeaponController.NowEquipedWeapon.CanAutoReload())
        {
            _storedWeaponController.WeaponFSM.SetState(WeaponController.WeaponState.Reload);
        }

        if (Input.GetMouseButtonUp(0))
        {
            _storedWeaponController.WeaponFSM.RevertToPreviousState();
        }
    }
}
