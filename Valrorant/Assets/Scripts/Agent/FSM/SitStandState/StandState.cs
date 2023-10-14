using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class StandState : IState
{
    Player storedPlayer;

    public StandState(Player player)
    {
        storedPlayer = player;
    }

    public void CheckStateChange()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            storedPlayer.SitStandFSM.SetState(Player.SitStandState.Sit);
        }
    }

    public void OnStateCollisionEnter(Collision collision)
    {
    }

    public void OnStateEnter()
    {
        storedPlayer.MovementComponent.LockToCrouchForce = false;
        storedPlayer.MovementComponent.Crouch(false);
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
