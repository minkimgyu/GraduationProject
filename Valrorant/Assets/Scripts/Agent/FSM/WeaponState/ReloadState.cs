using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class ReloadState : IState
{
    Player _storedPlayer;

    Timer reloadTimer = new Timer();
    Timer stateExitTimer = new Timer();

    public ReloadState(Player player)
    {
        _storedPlayer = player;
    }

    public void CheckStateChange()
    {
    }

    public void OnStateCollisionEnter(Collision collision)
    {
    }

    public void OnStateEnter()
    {
        if (_storedPlayer.WeaponHolder.NowEquipedWeapon.CanReload() == false)
        {
            _storedPlayer.WeaponFSM.RevertToPreviousState(); // 재장전을 할 수 없는 경우, 뒤로 돌아가기
        }

        _storedPlayer.WeaponHolder.NowEquipedWeapon.OnReload();

        reloadTimer.Start(_storedPlayer.WeaponHolder.NowEquipedWeapon.ReturnReloadFinishTime()); // 딜레이
        stateExitTimer.Start(_storedPlayer.WeaponHolder.NowEquipedWeapon.ReturnReloadStateExitTime()); // 딜레이
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
        reloadTimer.Update();
        if (reloadTimer.IsTimerFinish())
        {
            _storedPlayer.WeaponHolder.NowEquipedWeapon.ReloadAmmo();
        }

        stateExitTimer.Update();
        if (stateExitTimer.IsTimerFinish())
        {
            _storedPlayer.WeaponFSM.RevertToPreviousState();
        }
    }
}
