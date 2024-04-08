using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using Agent.Controller;

namespace Agent.States
{
    public class ReadyState : State
    {
        InteractionController _storedInteractionController;

        public ReadyState(InteractionController interactionController)
        {
            _storedInteractionController = interactionController;
        }

        public override void CheckStateChange()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (CanInteract() == false) return;

                _storedInteractionController.InteractionFSM.SetState(InteractionController.InteractionState.Interact);
            }
        }

        bool CanInteract()
        {
            return _storedInteractionController.InteractableTarget != null && _storedInteractionController.InteractableTarget.IsInteractable() == true;
        }

        public override void OnStateTriggerEnter(Collider collider)
        {
            IInteractable interactableTarget = collider.GetComponent<IInteractable>();
            if (interactableTarget.IsInteractable() == false) return; // IsInteractable 거짓이면 리턴

            if (_storedInteractionController.InteractableTarget != null)
            {
                _storedInteractionController.InteractableTarget.OnSightExit();
                _storedInteractionController.InteractableTarget = null;
            }

            _storedInteractionController.InteractableTarget = interactableTarget;
            _storedInteractionController.InteractableTarget.OnSightEnter();
        }

        public override void OnStateTriggerExit(Collider collision)
        {
            if (CanInteract() == false) return;

            _storedInteractionController.InteractableTarget.OnSightExit();
        }
    }

}