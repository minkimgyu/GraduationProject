using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FSM;

public class WeaponController : MonoBehaviour
{
    [SerializeField]
    Transform _cameraHolder;

    [SerializeField]
    GameObject[] _weapons;

    List<IWeaponContainer> _weaponsContainers = new List<IWeaponContainer>();
    public List<IWeaponContainer> WeaponsContainers { get { return _weaponsContainers; } }

    BaseWeapon _nowEquipedWeapon = null;
    public BaseWeapon NowEquipedWeapon { get { return _nowEquipedWeapon; } set { _nowEquipedWeapon = value; } }

    [SerializeField]
    Animator _ownerAnimator;

    int _weaponIndex = 0;
    public int WeaponIndex { get { return _weaponIndex; } set { _weaponIndex = value; } }

    public enum WeaponState
    {
        Idle,
        Equip,
        LeftAction,
        RightAction,
        Reload,
        Drop,
    }

    StateMachine<WeaponState> _weaponFSM;
    public StateMachine<WeaponState> WeaponFSM { get { return _weaponFSM; } }

    private void Start()
    {
        for (int i = 0; i < _weapons.Length; i++)
        {
            _weaponsContainers.Add(_weapons[i].GetComponent<IWeaponContainer>());

            if(i == 0) _nowEquipedWeapon = _weaponsContainers[i].ReturnWeapon();
            _weaponsContainers[i].ReturnWeapon().Initialize(gameObject, _cameraHolder, _ownerAnimator);
        }


        _weaponFSM = new StateMachine<WeaponState>();
        InitializeFSM();
    }

    void InitializeFSM()
    {
        Dictionary<WeaponState, IState> weaponStates = new Dictionary<WeaponState, IState>();

        IState idle = new IdleState(this);
        IState equip = new EquipState(this);
        IState reload = new ReloadState(this);
        IState drop = new DropState(this);

        IState leftAction = new LeftActionState(this);
        IState rightAction = new RightActionState(this);

        weaponStates.Add(WeaponState.Idle, idle);
        weaponStates.Add(WeaponState.Equip, equip);
        weaponStates.Add(WeaponState.Reload, reload);
        weaponStates.Add(WeaponState.Drop, drop);

        weaponStates.Add(WeaponState.LeftAction, leftAction);
        weaponStates.Add(WeaponState.RightAction, rightAction);

        _weaponFSM.Initialize(weaponStates, WeaponState.Equip);
    }

    private void Update()
    {
        _weaponFSM.OnUpdate();
    }

    private void FixedUpdate()
    {
        _weaponFSM.OnFixedUpdate();
    }

    private void LateUpdate()
    {
        _weaponFSM.OnLateUpdate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        _weaponFSM.OnCollisionEnter(collision);
    }
}
