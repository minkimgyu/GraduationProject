using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class ActionState : IState
{
    PlayerController _storedPlayer;

    public ActionState(PlayerController player)
    {
        _storedPlayer = player;
    }

    public void CheckStateChange()
    {
        bool canInteract = _storedPlayer.InteractionComponent.CanInteract();
        if (canInteract == false) _storedPlayer.InteractionFSM.SetState(PlayerController.InteractionState.Ready);
    }

    public void OnStateUpdate()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            _storedPlayer.InteractionComponent.InteractableTarget.OnInteract();
        }
    }

    public void OnStateEnter()
    {
        _storedPlayer.InteractionComponent.InteractableTarget.OnSightEnter();
        Debug.Log("OnSightEnter");
    }

    public void OnStateExit()
    {
        _storedPlayer.InteractionComponent.InteractableTarget.OnSightExit();
        Debug.Log("OnSightExit");
    }

    public void OnStateFixedUpdate()
    {
    }

    public void OnStateLateUpdate()
    {
    }

    public void OnStateCollisionEnter(Collision collision)
    {
    }
}
