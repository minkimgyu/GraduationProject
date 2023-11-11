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

    StateMachine<PostureState> _postureFSM;
    public StateMachine<PostureState> PostureFSM { get { return _postureFSM; } }

    StateMachine<MovementState> _movementFSM;
    public StateMachine<MovementState> MovementFSM { get { return _movementFSM; } }

    MovementComponent _movementComponent;
    public MovementComponent MovementComponent { get { return _movementComponent; } }

    ViewComponent _viewComponent;
    public ViewComponent ViewComponent { get { return _viewComponent; } }

    InteractionController _interactionComponent;
    public InteractionController InteractionComponent { get { return _interactionComponent; } }

    Animator _animator;

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _interactionComponent = GetComponent<InteractionController>();
        _movementComponent = GetComponent<MovementComponent>();
        _viewComponent = GetComponent<ViewComponent>();

        _movementFSM = new StateMachine<MovementState>();
        _postureFSM = new StateMachine<PostureState>();

        InitializeFSM();
    }

    private void Update()
    {
        _movementFSM.OnUpdate();
        _postureFSM.OnUpdate();
    }

    private void FixedUpdate()
    {
        _movementFSM.OnFixedUpdate();
        _postureFSM.OnFixedUpdate();
    }

    private void LateUpdate()
    {
        _movementFSM.OnLateUpdate();
        _postureFSM.OnLateUpdate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        _movementFSM.OnCollisionEnter(collision);
        _postureFSM.OnCollisionEnter(collision);
    }

    void InitializeFSM()
    {
        Dictionary<PostureState, BaseState> postureStates = new Dictionary<PostureState, BaseState>();

        BaseState sit = new SitState(this);
        BaseState stand = new StandState(this);

        postureStates.Add(PostureState.Sit, sit);
        postureStates.Add(PostureState.Stand, stand);

        _postureFSM.Initialize(postureStates);
        _postureFSM.SetState(PostureState.Stand);

        Dictionary<MovementState, BaseState> movementStates = new Dictionary<MovementState, BaseState>();

        BaseState stop = new StopState(this);
        BaseState walk = new WalkState(this);
        BaseState creep = new CreepState(this);
        BaseState jump = new JumpState(this);

        movementStates.Add(MovementState.Stop, stop);
        movementStates.Add(MovementState.Walk, walk);
        movementStates.Add(MovementState.Creep, creep);
        movementStates.Add(MovementState.Jump, jump);

        _movementFSM.Initialize(movementStates);
        _movementFSM.SetState(MovementState.Stop);
    }

    public void GetDamage(float damage)
    {
    }

    public Vector3 GetFowardVector()
    {
        return transform.forward;
    }
}
