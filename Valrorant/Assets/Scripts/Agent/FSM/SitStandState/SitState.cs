using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class SitState : State
{
    PlayerController _storedPlayer;

    public SitState(PlayerController player)
    {
        _storedPlayer = player;
    }

    public override void OnStateEnter()
    {
        _storedPlayer.MovementComponent.LockToCrouchForce = true;
        _storedPlayer.MovementComponent.SwitchPosture();
    }

    public override void OnStateUpdate()
    {
        _storedPlayer.MovementComponent.UpdateCrouch();
    }

    public override void CheckStateChange()
    {
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            _storedPlayer.PostureFSM.SetState(PlayerController.PostureState.Stand);
        }
    }
}
