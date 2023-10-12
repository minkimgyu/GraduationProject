using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class Player : MonoBehaviour
{
    public enum MovementState
    {
        Stop,
        Crouch,
        Walk,
        Creep,
        Jump
    }

    public enum WeaponState
    {
        Idle,
        Equip,
        Attack,
        Reload,
    }

    StateMachine<MovementState> _movementFSM;
    public StateMachine<MovementState> MovementFSM { get { return _movementFSM; } }

    StateMachine<WeaponState> _weaponFSM;
    public StateMachine<WeaponState> WeaponFSM { get { return _weaponFSM; } }


    MovementComponent movementComponent;
    public MovementComponent MovementComponent { get { return movementComponent; } }

    ViewComponent viewComponent;
    public ViewComponent ViewComponent { get { return viewComponent; } }

    private void Start()
    {
        movementComponent = GetComponent<MovementComponent>();
        viewComponent = GetComponent<ViewComponent>();
        viewComponent.SetZoom(false);

        _movementFSM = new StateMachine<MovementState>();
        _weaponFSM = new StateMachine<WeaponState>();
        InitializeFSM();
    }

    private void Update()
    {
        _movementFSM.DoUpdate();
        _weaponFSM.DoUpdate();
    }

    private void FixedUpdate()
    {
        _movementFSM.DoFixedUpdate();
        _weaponFSM.DoFixedUpdate();
    }

    private void LateUpdate()
    {
        _movementFSM.DoLateUpdate();
        _weaponFSM.DoLateUpdate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        _movementFSM.DoCollisionEnter(collision);
        _weaponFSM.DoCollisionEnter(collision);
    }

    void InitializeFSM()
    {
        Dictionary<MovementState, IState> movementStates = new Dictionary<MovementState, IState>();

        IState stop = new StopState(this);
        IState walk = new WalkState(this);
        IState creep = new CreepState(this);
        IState crouch = new CrouchState(this);
        IState jump = new JumpState(this);

        movementStates.Add(MovementState.Stop, stop);
        movementStates.Add(MovementState.Crouch, crouch);
        movementStates.Add(MovementState.Walk, walk);
        movementStates.Add(MovementState.Creep, creep);
        movementStates.Add(MovementState.Jump, jump);

        _movementFSM.Initialize(movementStates, MovementState.Stop);

        Dictionary<WeaponState, IState> weaponStates = new Dictionary<WeaponState, IState>();

        IState idle = new IdleState();
        IState equip = new EquipState();
        IState attack = new AttackState();
        IState reload = new ReloadState();

        weaponStates.Add(WeaponState.Idle, idle);
        weaponStates.Add(WeaponState.Equip, equip);
        weaponStates.Add(WeaponState.Attack, attack);
        weaponStates.Add(WeaponState.Reload, reload);

        _weaponFSM.Initialize(weaponStates, WeaponState.Idle);
    }
}
