using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class SitState : IState
{
    PlayerController _storedPlayer;

    public SitState(PlayerController player)
    {
        _storedPlayer = player;
    }

    public void OnStateCollisionEnter(Collision collision)
    {
    }

    public void OnStateEnter()
    {
        _storedPlayer.MovementComponent.LockToCrouchForce = true;
        _storedPlayer.MovementComponent.SwitchPosture();
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
        _storedPlayer.MovementComponent.UpdateCrouch();
    }

    public void CheckStateChange()
    {
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            _storedPlayer.PostureFSM.SetState(PlayerController.PostureState.Stand);
        }
    }
}
