using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class CrouchState : IState
{
    Player storedPlayer;

    public CrouchState(Player player)
    {
        storedPlayer = player;
    }

    public void OnStateCollisionEnter(Collision collision)
    {
    }

    public void OnStateEnter()
    {
        storedPlayer.MovementComponent.Crouch(true);
    }

    public void OnStateExit()
    {
        storedPlayer.MovementComponent.Crouch(false);
    }

    public void OnStateFixedUpdate()
    {
        storedPlayer.MovementComponent.Move(true);
    }

    public void OnStateLateUpdate()
    {
        storedPlayer.ViewComponent.ResetCamera();
    }

    public void OnStateUpdate()
    {
        storedPlayer.MovementComponent.ResetDirection();
        storedPlayer.ViewComponent.ResetView();
    }

    public void CheckStateChange()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            storedPlayer.MovementFSM.SetState(Player.MovementState.Jump);
        }

        if (Input.GetKey(KeyCode.LeftControl) == true) return;

        if (Input.GetAxisRaw("Horizontal") == 0 || Input.GetAxisRaw("Vertical") == 0)
        {
            storedPlayer.MovementFSM.SetState(Player.MovementState.Stop);
            
        }

        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            storedPlayer.MovementFSM.SetState(Player.MovementState.Walk);
        }
    }
}
