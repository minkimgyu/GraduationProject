using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class ActionState : IState
{
    InteractionController _storedInteractionController;

    WeaponController _weaponController;

    public ActionState(InteractionController interactionController)
    {
        _storedInteractionController = interactionController;

        _weaponController = interactionController.GetComponentInParent<WeaponController>();
    }

    public void CheckStateChange() { }

    public void OnStateUpdate() { }

    public void OnStateEnter() 
    {
        _storedInteractionController.InteractableTarget.OnSightExit();
        _storedInteractionController.InteractableTarget.OnInteract(_weaponController);

        _storedInteractionController.InteractableTarget = null;
        _storedInteractionController.InteractionFSM.RevertToPreviousState();
    }

    public void OnStateExit() { }

    public void OnStateFixedUpdate() { }

    public void OnStateLateUpdate() { }

    public void OnStateCollisionEnter(Collision collision) { }

    public void OnStateTriggerEnter(Collider collider) { }

    public void OnStateTriggerExit(Collider collider) { }
}
