using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class StandbyState : IState
{
    InteractionController _storedInteractionController;

    public StandbyState(InteractionController interactionController)
    {
        _storedInteractionController = interactionController;
    }

    public void CheckStateChange()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (CanInteract() == false) return;

            _storedInteractionController.InteractionFSM.SetState(InteractionController.InteractionState.Action);
        }
    }

    bool CanInteract()
    {
        return _storedInteractionController.InteractableTarget != null && _storedInteractionController.InteractableTarget.IsInteractable() == true;
    }

    public void OnStateTriggerEnter(Collider collider)
    {
        IInteractContainer interactContainer = collider.GetComponent<IInteractContainer>();
        if (interactContainer == null) return;

        IInteractable interactableTarget = interactContainer.ReturnInteractableObject();
        if (interactableTarget.IsInteractable() == false) return; // IsInteractable 거짓이면 리턴

        if (_storedInteractionController.InteractableTarget != null)
        {
            _storedInteractionController.InteractableTarget.OnSightExit();
            _storedInteractionController.InteractableTarget = null;
        }

        _storedInteractionController.InteractableTarget = interactableTarget;
        _storedInteractionController.InteractableTarget.OnSightEnter();
    }

    public void OnStateTriggerExit(Collider collision) 
    {
        if (CanInteract() == false) return;

        _storedInteractionController.InteractableTarget.OnSightExit();
    }

    public void OnStateCollisionEnter(Collision collision) { }

    public void OnStateEnter() { }

    public void OnStateExit() { }

    public void OnStateFixedUpdate() { }

    public void OnStateLateUpdate() { }

    public void OnStateUpdate() { }
}
