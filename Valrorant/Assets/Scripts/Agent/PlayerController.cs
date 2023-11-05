using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class PlayerController : MonoBehaviour, IDamageable
{
    public enum InteractionState
    {
        Ready,
        Action
    }

    public enum PostureState
    {
        Sit,
        Stand
    }

    public enum MovementState
    {
        Stop,
        Walk,
        Creep,
        Jump
    }

    

    StateMachine<InteractionState> _interactionFSM;
    public StateMachine<InteractionState> InteractionFSM { get { return _interactionFSM; } }

    StateMachine<PostureState> _postureFSM;
    public StateMachine<PostureState> PostureFSM { get { return _postureFSM; } }

    StateMachine<MovementState> _movementFSM;
    public StateMachine<MovementState> MovementFSM { get { return _movementFSM; } }

    MovementComponent _movementComponent;
    public MovementComponent MovementComponent { get { return _movementComponent; } }

    ViewComponent _viewComponent;
    public ViewComponent ViewComponent { get { return _viewComponent; } }

    InteractionComponent _interactionComponent;
    public InteractionComponent InteractionComponent { get { return _interactionComponent; } }

    Animator _animator;

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _interactionComponent = GetComponent<InteractionComponent>();
        _movementComponent = GetComponent<MovementComponent>();
        _viewComponent = GetComponent<ViewComponent>();

        _movementFSM = new StateMachine<MovementState>();
        _postureFSM = new StateMachine<PostureState>();
        _interactionFSM = new StateMachine<InteractionState>();

        InitializeFSM();
    }

    private void Update()
    {
        _movementFSM.OnUpdate();
        _postureFSM.OnUpdate();
        _interactionFSM.OnUpdate();
    }

    private void FixedUpdate()
    {
        _movementFSM.OnFixedUpdate();
        _postureFSM.OnFixedUpdate();
        _interactionFSM.OnFixedUpdate();
    }

    private void LateUpdate()
    {
        _movementFSM.OnLateUpdate();
        _postureFSM.OnLateUpdate();
        _interactionFSM.OnLateUpdate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        _movementFSM.OnCollisionEnter(collision);
        _postureFSM.OnCollisionEnter(collision);
        _interactionFSM.OnCollisionEnter(collision);
    }

    void InitializeFSM()
    {
        Dictionary<InteractionState, IState> interactionStates = new Dictionary<InteractionState, IState>();

        IState ready = new ReadyState(this);
        IState action = new ActionState(this);

        interactionStates.Add(InteractionState.Ready, ready);
        interactionStates.Add(InteractionState.Action, action);

        _interactionFSM.Initialize(interactionStates, InteractionState.Ready);


        Dictionary<PostureState, IState> postureStates = new Dictionary<PostureState, IState>();

        IState sit = new SitState(this);
        IState stand = new StandState(this);

        postureStates.Add(PostureState.Sit, sit);
        postureStates.Add(PostureState.Stand, stand);

        _postureFSM.Initialize(postureStates, PostureState.Stand);


        Dictionary<MovementState, IState> movementStates = new Dictionary<MovementState, IState>();

        IState stop = new StopState(this);
        IState walk = new WalkState(this);
        IState creep = new CreepState(this);
        IState jump = new JumpState(this);

        movementStates.Add(MovementState.Stop, stop);
        movementStates.Add(MovementState.Walk, walk);
        movementStates.Add(MovementState.Creep, creep);
        movementStates.Add(MovementState.Jump, jump);

        _movementFSM.Initialize(movementStates, MovementState.Stop);
    }

    public void GetDamage(float damage)
    {
    }

    public Vector3 GetFowardVector()
    {
        return transform.forward;
    }
}
