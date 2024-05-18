using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;
using Agent.States;

namespace Agent.Controller
{
    public interface IInteractable
    {
        void OnSightEnter();

        T ReturnComponent<T>(); // 오브젝트 리턴 후, 이걸 사용해서 GetComponent 진행

        void OnSightExit();

        bool IsInteractable();
    }

    public class InteractionController : MonoBehaviour
    {
        public enum State
        {
            Ready,
            Interact,
        }

        StateMachine<State> _interactionFSM;
        IInteractable _interactableTarget;

        IInteractable ReturnTarget() { return _interactableTarget; }
        void ResetTarget(IInteractable newTarget) { _interactableTarget = newTarget; }

        Action<BaseWeapon> SendWeaponToController;

        void SetState(State state) { _interactionFSM.SetState(state); }
        void RevertToPreviousState() { _interactionFSM.RevertToPreviousState(); }

        public void OnHandleInteract() => _interactionFSM.OnHandleInteract();

        public void Initialize()
        {
            WeaponController weaponController = GetComponentInParent<WeaponController>();
            SendWeaponToController = weaponController.OnWeaponReceived;

            _interactionFSM = new StateMachine<State>();
            Dictionary<State, BaseState> interactionStates = new Dictionary<State, BaseState>();

            BaseState ready = new ReadyState(SetState, ResetTarget, ReturnTarget);
            BaseState interact = new InteractState(RevertToPreviousState, SendWeaponToController, ResetTarget, ReturnTarget);

            interactionStates.Add(State.Ready, ready);
            interactionStates.Add(State.Interact, interact);

            _interactionFSM.Initialize(interactionStates);
            _interactionFSM.SetState(State.Ready);
        }

        private void OnTriggerEnter(Collider collider)
        {
            _interactionFSM.OnTriggerEnter(collider);
        }

        private void OnTriggerExit(Collider collider)
        {
            _interactionFSM.OnTriggerExit(collider);
        }
    }
}