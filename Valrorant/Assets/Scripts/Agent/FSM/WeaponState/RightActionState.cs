using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class RightActionState : IState
{
    Player storedPlayer;

    public RightActionState(Player player)
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
        storedPlayer.WeaponHolder.NowEquipedWeapon.OnSubActionStart();
    }

    public void OnStateExit()
    {
        storedPlayer.WeaponHolder.NowEquipedWeapon.OnSubActionEnd();
    }

    public void OnStateFixedUpdate()
    {
    }

    public void OnStateLateUpdate()
    {
    }

    public void OnStateUpdate()
    {
        storedPlayer.WeaponHolder.NowEquipedWeapon.OnSubActionProgress();
        if(Input.GetMouseButtonUp(1))
        {
            storedPlayer.WeaponFSM.RevertToPreviousState();
        }
    }
}
