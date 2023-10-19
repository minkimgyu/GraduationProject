using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class IdleState : IState
{
    Player _storedPlayer;

    public IdleState(Player player)
    {
        _storedPlayer = player;
    }

    public void CheckStateChange()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _storedPlayer.WeaponHolder.WeaponIndex = 0;
            _storedPlayer.WeaponFSM.SetState(Player.WeaponState.Equip);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _storedPlayer.WeaponHolder.WeaponIndex = 1;
            _storedPlayer.WeaponFSM.SetState(Player.WeaponState.Equip);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _storedPlayer.WeaponHolder.WeaponIndex = 2;
            _storedPlayer.WeaponFSM.SetState(Player.WeaponState.Equip);
        }

        if (Input.GetMouseButtonDown(0))
        {
            _storedPlayer.WeaponFSM.SetState(Player.WeaponState.LeftAction);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            _storedPlayer.WeaponFSM.SetState(Player.WeaponState.RightAction);
        }

        if (Input.GetKeyDown(KeyCode.R) && _storedPlayer.WeaponHolder.NowEquipedWeapon.CanReload())
        {
            _storedPlayer.WeaponFSM.SetState(Player.WeaponState.Reload);
        }
    }

    public void OnStateCollisionEnter(Collision collision)
    {
    }

    public void OnStateEnter()
    {
        if(_storedPlayer.WeaponHolder.NowEquipedWeapon.CheckNowReload())
        {
            _storedPlayer.WeaponFSM.SetState(Player.WeaponState.Reload);
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
