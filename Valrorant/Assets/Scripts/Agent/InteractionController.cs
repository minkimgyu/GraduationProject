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

        T ReturnComponent<T>(); // ������Ʈ ���� ��, �̰� ����ؼ� GetComponent ����

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

        private void Start()
        {
            _interactionFSM = new StateMachine<State>();
            InitializeFSM();

            WeaponController weaponController = GetComponentInParent<WeaponController>();
            SendWeaponToController = weaponController.OnWeaponReceived;
        }

        void InitializeFSM()
        {
            Dictionary<State, BaseState> interactionStates = new Dictionary<State, BaseState>();

            BaseState ready = new ReadyState(SetState, ResetTarget, ReturnTarget);
            BaseState interact = new InteractState(RevertToPreviousState, SendWeaponToController, ResetTarget, ReturnTarget);

            interactionStates.Add(State.Ready, ready);
            interactionStates.Add(State.Interact, interact);

            _interactionFSM.Initialize(interactionStates);
            _interactionFSM.SetState(State.Ready);
        }

        private void Update()
        {
            _interactionFSM.OnUpdate();
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