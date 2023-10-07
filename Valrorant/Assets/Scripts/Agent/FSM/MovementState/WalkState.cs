using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class WalkState : IState
{
    Player storedPlayer;

    public WalkState(Player player)
    {
        storedPlayer = player;
    }

    public void OnStateEnter()
    {
    }

    public void OnStateExit()
    {
    }

    public void OnStateUpdate()
    {
        storedPlayer.MovementComponent.ResetDirection();
        storedPlayer.ViewComponent.ResetView();

        ChangeState();
    }

    bool NowPressAlt()
    {
        return Input.GetKey(KeyCode.LeftAlt) == true || Input.GetKey(KeyCode.RightAlt) == true;
    }

    public void OnStateFixedUpdate()
    {
        bool pressAlt = NowPressAlt();
        storedPlayer.MovementComponent.Move(pressAlt);
    }

    public void OnStateLateUpdate()
    {
        storedPlayer.ViewComponent.ResetCamera();
    }

    public void ChangeState()
    {
        if (storedPlayer.MovementComponent.CanMove() == false) storedPlayer.MovementFSM.SetState(Player.MovementState.Stop);
        else if (storedPlayer.MovementComponent.CanJump()) storedPlayer.MovementFSM.SetState(Player.MovementState.Jump);
        else if (storedPlayer.MovementComponent.CanCrouch()) storedPlayer.MovementFSM.SetState(Player.MovementState.Crouch);
    }
}
