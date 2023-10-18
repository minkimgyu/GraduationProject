using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class Player : MonoBehaviour, IDamageable
{
    public enum SitStandState
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

    public enum WeaponState
    {
        Idle,
        Equip,
        LeftAction,
        RightAction,
        Reload,
    }

    StateMachine<SitStandState> _sitStandFSM;
    public StateMachine<SitStandState> SitStandFSM { get { return _sitStandFSM; } }

    StateMachine<MovementState> _movementFSM;
    public StateMachine<MovementState> MovementFSM { get { return _movementFSM; } }

    StateMachine<WeaponState> _weaponFSM;
    public StateMachine<WeaponState> WeaponFSM { get { return _weaponFSM; } }


    MovementComponent _movementComponent;
    public MovementComponent MovementComponent { get { return _movementComponent; } }

    ViewComponent _viewComponent;
    public ViewComponent ViewComponent { get { return _viewComponent; } }

    WeaponHoldComponent _weaponHoldComponent;
    public WeaponHoldComponent WeaponHolder { get { return _weaponHoldComponent; } }

    Animator _animator;

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _weaponHoldComponent = GetComponent<WeaponHoldComponent>();
        _movementComponent = GetComponent<MovementComponent>();
        _viewComponent = GetComponent<ViewComponent>();


        _movementFSM = new StateMachine<MovementState>();
        _weaponFSM = new StateMachine<WeaponState>();
        _sitStandFSM = new StateMachine<SitStandState>();

        _weaponHoldComponent.Initialize(gameObject, _viewComponent.FirePoint, _animator);
        InitializeFSM();
    }

    private void Update()
    {
        _movementFSM.OnUpdate();
        _sitStandFSM.OnUpdate();
        _weaponFSM.OnUpdate();
    }

    private void FixedUpdate()
    {
        _movementFSM.OnFixedUpdate();
        _sitStandFSM.OnFixedUpdate();
        _weaponFSM.OnFixedUpdate();
    }

    private void LateUpdate()
    {
        _movementFSM.OnLateUpdate();
        _sitStandFSM.OnLateUpdate();
        _weaponFSM.OnLateUpdate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        _movementFSM.OnCollisionEnter(collision);
        _sitStandFSM.OnCollisionEnter(collision);
        _weaponFSM.OnCollisionEnter(collision);
    }

    void InitializeFSM()
    {
        Dictionary<SitStandState, IState> sitStandStates = new Dictionary<SitStandState, IState>();

        IState sit = new SitState(this);
        IState stand = new StandState(this);

        sitStandStates.Add(SitStandState.Sit, sit);
        sitStandStates.Add(SitStandState.Stand, stand);

        _sitStandFSM.Initialize(sitStandStates, SitStandState.Stand);


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

        Dictionary<WeaponState, IState> weaponStates = new Dictionary<WeaponState, IState>();

        IState idle = new IdleState(this);
        IState equip = new EquipState(this);
        IState reload = new ReloadState(this);

        IState leftAction = new LeftActionState(this);
        IState rightAction = new RightActionState(this);

        weaponStates.Add(WeaponState.Idle, idle);
        weaponStates.Add(WeaponState.Equip, equip);
        weaponStates.Add(WeaponState.Reload, reload);

        weaponStates.Add(WeaponState.LeftAction, leftAction);
        weaponStates.Add(WeaponState.RightAction, rightAction);

        _weaponFSM.Initialize(weaponStates, WeaponState.Equip);
    }
    public void GetDamage(float damage)
    {
    }

    public Vector3 GetFowardVector()
    {
        return transform.forward;
    }
}
