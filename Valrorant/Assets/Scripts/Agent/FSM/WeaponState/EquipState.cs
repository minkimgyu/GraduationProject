using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

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

    void ChangeWeapon()
    {
        for (int i = 0; i < _storedWeaponController.WeaponsContainers.Count; i++)
        {
            if (i == _storedWeaponController.WeaponIndex)
            {
                _storedWeaponController.NowEquipedWeapon = _storedWeaponController.WeaponsContainers[_storedWeaponController.WeaponIndex].ReturnWeapon();
                _storedWeaponController.NowEquipedWeapon.OnEquip();
                // ������ �Ǿ��� ���� ��������ش�.
            }
            else
            {
                _storedWeaponController.WeaponsContainers[i].ReturnWeapon().OnUnEquip();
                // �̶��� ���������ش�.
            }
        }
    }

    public void OnStateEnter()
    {
        ChangeWeapon();
        _timer.Start(_storedWeaponController.NowEquipedWeapon.EquipFinishTime); // ������
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
        _timer.Update();
    }
}
