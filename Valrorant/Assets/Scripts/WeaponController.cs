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
    GameObject armMesh;

    [SerializeField]
    Transform _weaponParent;

    [SerializeField]
    float _weaponThrowPower = 3;

    Dictionary<BaseWeapon.Type, BaseWeapon> _weaponsContainers = new Dictionary<BaseWeapon.Type, BaseWeapon>();
    public Dictionary<BaseWeapon.Type, BaseWeapon> WeaponsContainers { get { return _weaponsContainers; } }

    BaseWeapon _nowEquipedWeapon = null;
    public BaseWeapon NowEquipedWeapon { get { return _nowEquipedWeapon; } set { _nowEquipedWeapon = value; } }

    [SerializeField]
    Animator _ownerAnimator;

    BaseWeapon.Type _nowEquipedweaponType;
    public BaseWeapon.Type NowEquipedweaponType { get { return _nowEquipedweaponType; } set { _nowEquipedweaponType = value; } }

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

    public Action<float> OnWeaponChangeRequested;

    public bool CanChangeWeapon(BaseWeapon.Type weaponType)
    {
        return _weaponsContainers.ContainsKey(weaponType);
    }

    void InitializeWeapons()
    {
        IWeaponContainer[] containers = _weaponParent.GetComponentsInChildren<IWeaponContainer>();
        for (int i = 0; i < containers.Length; i++)
        {
            BaseWeapon baseWeapon = containers[i].ReturnWeapon();
            baseWeapon.Initialize(gameObject, armMesh, _cameraHolder, _ownerAnimator);

            _weaponsContainers.Add(baseWeapon.WeaponType, baseWeapon);
        }

        _nowEquipedweaponType = BaseWeapon.Type.Melee;
    }

    void InitializeEvent()
    {
        MovementComponent movementComponent = GetComponent<MovementComponent>();
        OnWeaponChangeRequested = movementComponent.OnWeaponChangeRequested; // 할당
    }

    public void AddWeapon(BaseWeapon baseWeapon)
    {
        baseWeapon.transform.SetParent(_weaponParent);
        baseWeapon.PositionWeaponMesh(false);

        baseWeapon.OnRooting(gameObject, _cameraHolder, _ownerAnimator);
        _weaponsContainers.Add(baseWeapon.WeaponType, baseWeapon);

        if (baseWeapon.WeaponType == _nowEquipedWeapon.WeaponType)
        {
            DropWeapon();
            _nowEquipedWeapon = baseWeapon;
            _weaponFSM.SetState(WeaponState.Equip);
        }
    }

    private void Start()
    {
        InitializeEvent();
        InitializeWeapons();

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

    #region WeaponDrop

    public void DropWeapon()
    {
        _nowEquipedWeapon.ThrowGun(_weaponThrowPower);
        _nowEquipedWeapon.OnDrop(); // 드랍 함수 호출
        _weaponsContainers.Remove(_nowEquipedweaponType); // 현재 끼고있는 무기를 제거

        _nowEquipedWeapon = null; // 현재 장착하고 있는 무기를 null로 바꿔줌
    }

    public void ResetWeaponTypeWhenDrop()
    {
        if (_nowEquipedweaponType == BaseWeapon.Type.Main)
        {
            if(_weaponsContainers.ContainsKey(BaseWeapon.Type.Sub))
            {
                _nowEquipedweaponType = BaseWeapon.Type.Sub;
            }
            else
            {
                _nowEquipedweaponType = BaseWeapon.Type.Melee;
            }
        }
        else if (_nowEquipedweaponType == BaseWeapon.Type.Sub) _nowEquipedweaponType = BaseWeapon.Type.Melee;
    }

    #endregion

    #region WeaponChange

    public void ChangeWeapon()
    {
        BaseWeapon.Type nowType = _nowEquipedweaponType;

        if (_nowEquipedWeapon != null)
        {
            _nowEquipedWeapon.OnUnEquip();
        }

        _nowEquipedWeapon = _weaponsContainers[nowType];
        _weaponsContainers[nowType].OnEquip();

        ActivateEquipedWeapon();

        OnWeaponChangeRequested?.Invoke(_nowEquipedWeapon.SlowDownRatioByWeaponWeight);
    }

    void ActivateEquipedWeapon()
    {
        foreach (var weapon in _weaponsContainers)
        {
            if (weapon.Key == _nowEquipedweaponType)
            {
                weapon.Value.gameObject.SetActive(true);
            }
            else
            {
                weapon.Value.gameObject.SetActive(false);
            }
        }
    }

    #endregion
}
