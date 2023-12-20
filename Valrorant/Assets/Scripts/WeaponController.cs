using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FSM;

public interface IEquipedWeapon
{
    BaseWeapon ReturnNowEquipedWeapon();
}

public class WeaponController : MonoBehaviour, IEquipedWeapon
{
    [SerializeField]
    Transform _cameraHolder;

    [SerializeField]
    GameObject armMesh;

    [SerializeField]
    Transform _weaponParent;

    [SerializeField]
    float _weaponThrowPower = 3;



    BaseWeapon _storedWeaponWhenInteracting; // Interacting을 위해 해당 무기를 잠깐 저장해두는 변수
    public BaseWeapon StoredWeaponWhenInteracting { get { return _storedWeaponWhenInteracting; } set { _storedWeaponWhenInteracting = value; } }

    BaseWeapon _nowEquipedWeapon = null;
    public BaseWeapon NowEquipedWeapon { get { return _nowEquipedWeapon; } set { _nowEquipedWeapon = value; } }



    Dictionary<BaseWeapon.Type, BaseWeapon> _weaponsContainers = new Dictionary<BaseWeapon.Type, BaseWeapon>();
    public Dictionary<BaseWeapon.Type, BaseWeapon> WeaponsContainers { get { return _weaponsContainers; } }

   

    [SerializeField]
    Animator _ownerAnimator;
    public Animator OwnerAnimator { get { return _ownerAnimator; } }

    public enum WeaponState
    {
        Idle,
        Equip,
        LeftAction,
        RightAction,
        Reload,

        Root,
        Drop
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
    }

    void InitializeEvent()
    {
        MovementComponent movementComponent = GetComponent<MovementComponent>();
        OnWeaponChangeRequested = movementComponent.OnWeaponChangeRequested; // 할당
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
        Dictionary<WeaponState, BaseState> weaponStates = new Dictionary<WeaponState, BaseState>();

        BaseState idle = new IdleState(this);
        BaseState equip = new EquipState(this);
        BaseState reload = new ReloadState(this);

        BaseState leftAction = new LeftActionState(this);
        BaseState rightAction = new RightActionState(this);

        BaseState root = new RootState(this);
        BaseState drop = new DropState(this);

        weaponStates.Add(WeaponState.Idle, idle);
        weaponStates.Add(WeaponState.Equip, equip);
        weaponStates.Add(WeaponState.Reload, reload);

        weaponStates.Add(WeaponState.LeftAction, leftAction);
        weaponStates.Add(WeaponState.RightAction, rightAction);

        weaponStates.Add(WeaponState.Root, root);
        weaponStates.Add(WeaponState.Drop, drop);

        _weaponFSM.Initialize(weaponStates);
        _weaponFSM.SetState(WeaponState.Equip, BaseWeapon.Type.Melee); // 초기 State 설정
    }

    private void Update()
    {
        _weaponFSM.OnUpdate();
        foreach (var weapon in _weaponsContainers)
        {
            weapon.Value.RunUpdateInController(); // 무기 루틴 돌려주기
        }
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

    #region WeaponInteracting

    public bool IsInteractingAndEquipedWeaponSameType() { return _storedWeaponWhenInteracting.WeaponType == _nowEquipedWeapon.WeaponType; }

    #endregion

    #region WeaponDrop


    bool DropWeapon(BaseWeapon weapon, bool activateWeapon = false)
    {
        if (weapon.CanDrop() == false) return false;

        weapon.ThrowGun(_weaponThrowPower);

        if (activateWeapon == true) weapon.gameObject.SetActive(true);
        weapon.OnDrop(); // 드랍 함수 호출

        _weaponsContainers.Remove(weapon.WeaponType);
        return true;
    }


    public bool DropWeaponSameTypeWithNowInteracting()
    {
        bool canDrop = DropWeapon(WeaponsContainers[_storedWeaponWhenInteracting.WeaponType], true);
        return canDrop;
    }

    public bool DropNowEquipedWeapon()
    {
        bool canDrop = DropWeapon(_nowEquipedWeapon);

        _nowEquipedWeapon = null; // 현재 장착하고 있는 무기를 null로 바꿔줌
        return canDrop;
    }

    public BaseWeapon.Type ReturnNextEquipWeaponTypeWhenDrop()
    {
        if (_nowEquipedWeapon.WeaponType == BaseWeapon.Type.Main)
        {
            if (_weaponsContainers.ContainsKey(BaseWeapon.Type.Sub))
            {
                return BaseWeapon.Type.Sub;
            }
            else
            {
                return BaseWeapon.Type.Melee;
            }
        }
        else
        {
            return BaseWeapon.Type.Melee;
        }
    }

    #endregion

    #region WeaponDrop

    public void CheckChangeStateForRooting()
    {
        if (IsInteractingWeaponExist()) // 할당된 경우
        {
            if (CheckContainerHaveSameType()) // 같은 종류의 무기를 이미 가지고 있는 경우
            {
                _weaponFSM.SetState(WeaponState.Drop, true);
            }
            else
            {
                _weaponFSM.SetState(WeaponState.Root);
            }
        }
    }

    public bool CheckContainerHaveSameType()
    {
        return _weaponsContainers.ContainsKey(_storedWeaponWhenInteracting.WeaponType);
    }

    #endregion


    #region AddingWeapon

    public void AddWeapon(BaseWeapon weapon)
    {
        weapon.Initialize(gameObject, armMesh, _cameraHolder, _ownerAnimator);
        _storedWeaponWhenInteracting = weapon;
    }

    #endregion

    #region WeaponInteracting

    public bool IsInteractingWeaponExist() { return _storedWeaponWhenInteracting != null; }

    #endregion

    #region WeaponRooting

    void AttachWeaponToArm(BaseWeapon weapon)
    {
        weapon.transform.SetParent(_weaponParent);
        weapon.PositionWeaponMesh(false);
    }

    public BaseWeapon.Type RootWeaponAndReturnType()
    {
        _storedWeaponWhenInteracting.OnRooting(gameObject, _cameraHolder, _ownerAnimator);
        _weaponsContainers.Add(_storedWeaponWhenInteracting.WeaponType, _storedWeaponWhenInteracting);

        AttachWeaponToArm(_storedWeaponWhenInteracting);

        BaseWeapon.Type tmpType = _storedWeaponWhenInteracting.WeaponType;
        _storedWeaponWhenInteracting = null;

        return tmpType;
    }

    #endregion

    #region WeaponChange

    public void GetWeaponChangeInput()
    {
        // 이부분은 타입을 보고 장착하도록 변경해준다.
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _weaponFSM.SetState(WeaponState.Equip, BaseWeapon.Type.Main);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _weaponFSM.SetState(WeaponState.Equip, BaseWeapon.Type.Sub);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _weaponFSM.SetState(WeaponState.Equip, BaseWeapon.Type.Melee);
        }
    }

    public void ChangeWeapon(BaseWeapon.Type weaponType)
    {
        if (_nowEquipedWeapon != null)
        {
            _nowEquipedWeapon.OnUnEquip();
        }

        _nowEquipedWeapon = _weaponsContainers[weaponType];
        _weaponsContainers[weaponType].OnEquip();

        ActivateEquipedWeapon();

        OnWeaponChangeRequested?.Invoke(_nowEquipedWeapon.SlowDownRatioByWeaponWeight);
    }

    void ActivateEquipedWeapon()
    {
        foreach (var weapon in _weaponsContainers)
        {
            if (weapon.Key == _nowEquipedWeapon.WeaponType)
            {
                weapon.Value.gameObject.SetActive(true);
            }
            else
            {
                weapon.Value.gameObject.SetActive(false);
            }
        }
    }

    public BaseWeapon ReturnNowEquipedWeapon()
    {
        return _nowEquipedWeapon;
    }

    #endregion
}
