using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class StandState : IState
{
    PlayerController _storedPlayer;

    public StandState(PlayerController player)
    {
        _storedPlayer = player;
    }

    public void CheckStateChange()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            _storedPlayer.PostureFSM.SetState(PlayerController.PostureState.Sit);
        }
    }

    public void OnStateCollisionEnter(Collision collision) { }

    public void OnStateEnter()
    {
        _storedPlayer.MovementComponent.LockToCrouchForce = false;
        _storedPlayer.MovementComponent.SwitchPosture();
    }

    public void OnStateExit() { }

    public void OnStateFixedUpdate() { }

    public void OnStateLateUpdate() { }

    public void OnStateTriggerEnter(Collider collider) { }

    public void OnStateTriggerExit(Collider collider) { }
    public void OnStateUpdate()
    {
        _storedPlayer.MovementComponent.UpdateStand();
    }
}
