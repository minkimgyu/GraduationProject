using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class StandState : State
{
    PlayerController _storedPlayer;

    public StandState(PlayerController player)
    {
        _storedPlayer = player;
    }

    public override void CheckStateChange()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            _storedPlayer.PostureFSM.SetState(PlayerController.PostureState.Sit);
        }
    }

    public override void OnStateEnter()
    {
        _storedPlayer.MovementComponent.LockToCrouchForce = false;
        _storedPlayer.MovementComponent.SwitchPosture();
    }

    public override void OnStateUpdate()
    {
        _storedPlayer.MovementComponent.UpdateStand();
    }
}
