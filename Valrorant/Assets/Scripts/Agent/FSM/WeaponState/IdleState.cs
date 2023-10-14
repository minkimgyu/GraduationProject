using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class IdleState : IState
{
    Player storedPlayer;

    public IdleState(Player player)
    {
        storedPlayer = player;
    }

    public void CheckStateChange()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            storedPlayer.WeaponHolder.WeaponIndex = 0;
            storedPlayer.WeaponFSM.SetState(Player.WeaponState.Equip);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            storedPlayer.WeaponHolder.WeaponIndex = 1;
            storedPlayer.WeaponFSM.SetState(Player.WeaponState.Equip);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            storedPlayer.WeaponHolder.WeaponIndex = 2;
            storedPlayer.WeaponFSM.SetState(Player.WeaponState.Equip);
        }

        if (Input.GetMouseButtonDown(0))
        {
            storedPlayer.WeaponFSM.SetState(Player.WeaponState.LeftAction);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            storedPlayer.WeaponFSM.SetState(Player.WeaponState.RightAction);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            storedPlayer.WeaponFSM.SetState(Player.WeaponState.Reload);
        }
    }

    public void OnStateCollisionEnter(Collision collision)
    {
    }

    public void OnStateEnter()
    {
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
