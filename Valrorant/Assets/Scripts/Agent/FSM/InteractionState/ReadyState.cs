using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class ReadyState : IState
{
    PlayerController _storedPlayer;

    public ReadyState(PlayerController player)
    {
        _storedPlayer = player;
    }

    public void CheckStateChange()
    {
        bool canInteract = _storedPlayer.InteractionComponent.CanInteract();
        if (canInteract == true) _storedPlayer.InteractionFSM.SetState(PlayerController.InteractionState.Action);
    }

    public void OnStateCollisionEnter(Collision collision)
    {
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
    }

    public void OnStateUpdate()
    {
    }
}
