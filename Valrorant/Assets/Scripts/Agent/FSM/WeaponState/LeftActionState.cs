using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class LeftActionState : IState
{
    Player storedPlayer;

    public LeftActionState(Player player)
    {
        storedPlayer = player;
    }

    public void CheckStateChange()
    {
    }

    public void OnStateCollisionEnter(Collision collision)
    {
    }

    public void OnStateEnter()
    {
        storedPlayer.WeaponHolder.NowEquipedWeapon.StartMainAction();
    }

    public void OnStateExit()
    {
        storedPlayer.WeaponHolder.NowEquipedWeapon.EndMainAction();
    }

    public void OnStateFixedUpdate()
    {
    }

    public void OnStateLateUpdate()
    {
    }

    public void OnStateUpdate()
    {
        storedPlayer.WeaponHolder.NowEquipedWeapon.ProgressMainAction();
        if (Input.GetMouseButtonUp(0))
        {
            storedPlayer.WeaponFSM.RevertToPreviousState();
        }
    }
}
