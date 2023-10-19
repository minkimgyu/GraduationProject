using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class StopState : IState
{
    Player storedPlayer;

    public StopState(Player player)
    {
        storedPlayer = player;
    }

    public void CheckStateChange()
    {
        if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) && Input.GetKeyDown(KeyCode.LeftAlt))
        {
            storedPlayer.MovementFSM.SetState(Player.MovementState.Creep);
        }

        if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0))
        {
            storedPlayer.MovementFSM.SetState(Player.MovementState.Walk);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            storedPlayer.MovementFSM.SetState(Player.MovementState.Jump);
        }
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
        storedPlayer.ViewComponent.ResetCamera();
    }

    public void OnStateUpdate()
    {
        storedPlayer.ViewComponent.ResetView();
        storedPlayer.MovementComponent.RaiseDisplacementEvent();
    }

    public void OnStateCollisionEnter(Collision collision)
    {
    }
}
