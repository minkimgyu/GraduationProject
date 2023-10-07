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

    void ChangeState()
    {
        if (storedPlayer.MovementComponent.CanMove()) storedPlayer.MovementFSM.SetState(Player.MovementState.Walk);
        else if (storedPlayer.MovementComponent.CanJump()) storedPlayer.MovementFSM.SetState(Player.MovementState.Jump);
        else if (storedPlayer.MovementComponent.CanCrouch()) storedPlayer.MovementFSM.SetState(Player.MovementState.Crouch);
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
        ChangeState();
    }
}
