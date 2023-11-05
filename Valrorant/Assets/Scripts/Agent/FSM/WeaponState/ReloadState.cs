using FSM;
using UnityEngine;

public class ReloadState : IState
{
    WeaponController _storedWeaponController;

    public ReloadState(WeaponController player)
    {
        _storedWeaponController = player;
    }

    public void CheckStateChange()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _storedWeaponController.WeaponIndex = 0;
            CancelReload();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _storedWeaponController.WeaponIndex = 1;
            CancelReload();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _storedWeaponController.WeaponIndex = 2;
            CancelReload();
        }

        bool canExit = _storedWeaponController.NowEquipedWeapon.IsReloadFinish();
        if (canExit)
        {
            _storedWeaponController.NowEquipedWeapon.ResetReload();
            _storedWeaponController.WeaponFSM.SetState(WeaponController.WeaponState.Idle);
        }
    }

    void CancelReload()
    {
        _storedWeaponController.NowEquipedWeapon.ResetReload();
        _storedWeaponController.WeaponFSM.SetState(WeaponController.WeaponState.Equip);
    }

    public void OnStateEnter()
    {
        _storedWeaponController.NowEquipedWeapon.OnReload();
    }

    public void OnStateCollisionEnter(Collision collision) { }

    public void OnStateExit() { }

    public void OnStateFixedUpdate() { }

    public void OnStateLateUpdate() { }

    public void OnStateUpdate() { }
}
