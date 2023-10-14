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
        storedPlayer.WeaponHolder.NowEquipedWeapon.OnMainActionStart();
    }

    public void OnStateExit()
    {
        storedPlayer.WeaponHolder.NowEquipedWeapon.OnMainActionEnd();
    }

    public void OnStateFixedUpdate()
    {
    }

    public void OnStateLateUpdate()
    {
    }

    public void OnStateUpdate()
    {
        storedPlayer.WeaponHolder.NowEquipedWeapon.OnMainActionProgress();
        if (Input.GetMouseButtonUp(0))
        {
            storedPlayer.WeaponFSM.RevertToPreviousState();
        }
    }
}
