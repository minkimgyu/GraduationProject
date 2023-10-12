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

    public void OnStateFixedUpdate()
    {
        storedPlayer.MovementComponent.Move();
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
        if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
        {
            storedPlayer.MovementFSM.SetState(Player.MovementState.Stop);
        }

        if (((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)) && Input.GetKeyDown(KeyCode.LeftAlt))
        {
            storedPlayer.MovementFSM.SetState(Player.MovementState.Creep);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            storedPlayer.MovementFSM.SetState(Player.MovementState.Jump);
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            storedPlayer.MovementFSM.SetState(Player.MovementState.Crouch);
        }
    }

    public void OnStateCollisionEnter(Collision collision)
    {
    }
}
