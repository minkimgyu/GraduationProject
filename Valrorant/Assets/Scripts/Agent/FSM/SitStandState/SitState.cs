using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class SitState : IState
{
    Player storedPlayer;

    public SitState(Player player)
    {
        storedPlayer = player;
    }

    public void OnStateCollisionEnter(Collision collision)
    {
    }

    public void OnStateEnter()
    {
        storedPlayer.MovementComponent.LockToCrouchForce = true;
        storedPlayer.MovementComponent.Crouch(true);
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

    public void CheckStateChange()
    {
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            storedPlayer.SitStandFSM.SetState(Player.SitStandState.Stand);
        }
    }
}
