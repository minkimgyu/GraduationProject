using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using Agent.Controller;
using System;

namespace Agent.States
{
    public class ReadyState : State
    {
        Action<InteractionController.State> SetState;
        Action<IInteractable> ResetTarget;
        Func<IInteractable> ReturnTarget;

        public ReadyState(Action<InteractionController.State> SetState, Action<IInteractable> ResetTarget, Func<IInteractable> ReturnTarget)
        {
            this.SetState = SetState;
            this.ResetTarget = ResetTarget;
            this.ReturnTarget = ReturnTarget;
        }

        public override void OnHandleInteract()
        {
            if (CanInteract() == false) return;
            SetState?.Invoke(InteractionController.State.Interact);
        }

        //public override void CheckStateChange()
        //{
        //    if (Input.GetKeyDown(KeyCode.F))
        //    {
                
        //    }
        //}

        bool CanInteract()
        {
            IInteractable target = ReturnTarget();
            return target != null && target.IsInteractable() == true;
        }

        public override void OnStateTriggerEnter(Collider collider)
        {
            IInteractable newTarget = collider.GetComponent<IInteractable>();
            if (newTarget == null || newTarget.IsInteractable() == false) return; // IsInteractable 거짓이면 리턴

            IInteractable oldTarget = ReturnTarget();
            if (oldTarget != null)
            {
                oldTarget.OnSightExit();
                ResetTarget(null);
            }

            ResetTarget(newTarget);

            if (oldTarget != null) oldTarget.OnSightEnter();
        }

        public override void OnStateTriggerExit(Collider collision)
        {
            if (CanInteract() == false) return;

            IInteractable oldTarget = ReturnTarget();
            if (oldTarget != null) oldTarget.OnSightExit();
        }
    }

}