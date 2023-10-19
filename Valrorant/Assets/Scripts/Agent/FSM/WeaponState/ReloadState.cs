using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class ReloadState : IState
{
    Player _storedPlayer;

    Timer _reloadTimer;
    Timer _stateExitTimer;

    public ReloadState(Player player)
    {
        _storedPlayer = player;
        _reloadTimer = new Timer();
        _stateExitTimer = new Timer();
    }

    public void CheckStateChange()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _storedPlayer.WeaponHolder.WeaponIndex = 0;
            StopReload();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _storedPlayer.WeaponHolder.WeaponIndex = 1;
            StopReload();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _storedPlayer.WeaponHolder.WeaponIndex = 2;
            StopReload();
        }
    }

    void StopReload()
    {
        _reloadTimer.Stop();
        _stateExitTimer.Stop();
        _storedPlayer.WeaponFSM.SetState(Player.WeaponState.Equip);
    }

    public void OnStateCollisionEnter(Collision collision)
    {
    }

    public void OnStateEnter()
    {
        _storedPlayer.WeaponHolder.NowEquipedWeapon.OnReload();
        _reloadTimer.Start(_storedPlayer.WeaponHolder.NowEquipedWeapon.ReturnReloadFinishTime()); // µÙ∑π¿Ã
        _stateExitTimer.Start(_storedPlayer.WeaponHolder.NowEquipedWeapon.ReturnReloadStateExitTime()); // µÙ∑π¿Ã
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
        _reloadTimer.Update();
        if (_reloadTimer.IsTimerFinish())
        {
            _storedPlayer.WeaponHolder.NowEquipedWeapon.ReloadAmmo();
        }

        _stateExitTimer.Update();
        if (_stateExitTimer.IsTimerFinish())
        {
            _storedPlayer.WeaponFSM.SetState(Player.WeaponState.Idle);
        }
    }
}
