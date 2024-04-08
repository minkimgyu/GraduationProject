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
        public enum InteractionState
        {
            Ready,
            Interact,
        }

        StateMachine<InteractionState> _interactionFSM;
        public StateMachine<InteractionState> InteractionFSM { get { return _interactionFSM; } }

        IInteractable _interactableTarget;
        public IInteractable InteractableTarget { get { return _interactableTarget; } set { _interactableTarget = value; } }

        private void Start()
        {
            _interactionFSM = new StateMachine<InteractionState>();
            InitializeFSM();
        }

        void InitializeFSM()
        {
            Dictionary<InteractionState, BaseState> interactionStates = new Dictionary<InteractionState, BaseState>();

            BaseState ready = new ReadyState(this);
            BaseState interact = new InteractState(this);


            interactionStates.Add(InteractionState.Ready, ready);
            interactionStates.Add(InteractionState.Interact, interact);

            _interactionFSM.Initialize(interactionStates);
            _interactionFSM.SetState(InteractionState.Ready);
        }

        private void Update()
        {
            _interactionFSM.OnUpdate();
        }

        private void FixedUpdate()
        {
            _interactionFSM.OnFixedUpdate();
        }

        private void LateUpdate()
        {
            _interactionFSM.OnLateUpdate();
        }

        private void OnCollisionEnter(Collision collision)
        {
            _interactionFSM.OnCollisionEnter(collision);
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