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
            _storedWeaponController.NowEquipedweaponType = BaseWeapon.Type.Main;
            CancelReload();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _storedWeaponController.NowEquipedweaponType = BaseWeapon.Type.Sub;
            CancelReload();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _storedWeaponController.NowEquipedweaponType = BaseWeapon.Type.Melee;
            CancelReload();
        }

        bool nowCancelMainAction = _storedWeaponController.NowEquipedWeapon.CancelReloadAndGoToMainAction();
        if (nowCancelMainAction)
        {
            _storedWeaponController.NowEquipedWeapon.ResetReload();
            _storedWeaponController.WeaponFSM.SetState(WeaponController.WeaponState.LeftAction);
        }

        bool nowCancelSubAction = _storedWeaponController.NowEquipedWeapon.CancelReloadAndGoToSubAction();
        if (nowCancelSubAction)
        {
            _storedWeaponController.NowEquipedWeapon.ResetReload();
            _storedWeaponController.WeaponFSM.SetState(WeaponController.WeaponState.RightAction);
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

    public void OnStateTriggerEnter(Collider collider) { }

    public void OnStateTriggerExit(Collider collider) { }
}
