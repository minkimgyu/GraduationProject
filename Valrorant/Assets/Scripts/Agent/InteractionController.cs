using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;

public interface IInteractable
{
    void OnSightEnter();

    void OnInteract(WeaponController weaponController);

    void OnSightExit();

    bool IsInteractable();
}

public class InteractionController : MonoBehaviour
{
    public enum InteractionState
    {
        Standby,
        Action
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
        Dictionary<InteractionState, IState> interactionStates = new Dictionary<InteractionState, IState>();

        IState standby = new StandbyState(this);
        IState action = new ActionState(this);

        interactionStates.Add(InteractionState.Standby, standby);
        interactionStates.Add(InteractionState.Action, action);

        _interactionFSM.Initialize(interactionStates, InteractionState.Standby);
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
