//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class BattleComponent : MonoBehaviour//, IEquipedWeapon
//{
//    BaseWeapon _nowEquipedWeapon;

//    Animator _ownerAnimator;

//    [SerializeField] Transform _weaponParent;
//    [SerializeField] GameObject _armMesh;
//    [SerializeField] Transform _cameraHolder;

//    Timer _attackTimer;
//    float attackDuration = 1.5f;

//    public BaseWeapon ReturnNowEquipedWeapon()
//    {
//        return _nowEquipedWeapon;
//    }

//    private void Start()
//    {
//        _ownerAnimator = GetComponentInChildren<Animator>();
//        _attackTimer = new Timer();

//        InitializeWeapons();
//    }

//    //public bool NeedToReload() { return _nowEquipedWeapon.NeedToReload(); }

//    public bool IsAttackFinish()
//    {
//        return _attackTimer.IsFinish;
//    }

//    public void ResetAttack()
//    {
//        _attackTimer.Reset();
//        _ownerAnimator.Play("AKIdle");
//    }

//    //public bool NowNeedToRefillAmmo() { return _nowEquipedWeapon.NowNeedToRefillAmmo(); }

//    //public bool CanAttack() { return _nowEquipedWeapon.CanAttack(); }

//    public void Attack()
//    {
//        _attackTimer.Update();

//        // 앞선 상황에서 Finish된 경우
//        if (_attackTimer.CanStart())
//        {
//            _nowEquipedWeapon.OnLeftClickStart();
//        }

//        _attackTimer.Start(attackDuration); // 여기서 시작

//        if (_attackTimer.IsRunning)
//        {
//            _nowEquipedWeapon.OnLeftClickProcess();
//        }

//        if (_attackTimer.IsFinish)
//        {
//            _nowEquipedWeapon.OnLeftClickEnd();
//        }
//    }

//    public bool IsReloadFinish()
//    {
//        return _nowEquipedWeapon.IsReloadFinish();
//    }

//    //public bool IsReloadRunning()
//    //{
//    //    return _nowEquipedWeapon.IsReloadRunning();
//    //}

//    public void ResetReload()
//    {
//        _nowEquipedWeapon.ResetReload();
//    }

//    public void Reload()
//    {
//        //_nowEquipedWeapon.OnReload();
//    }

//    public void FireEnd()
//    {
//        _nowEquipedWeapon.OnLeftClickEnd();
//    }

//    private void Update()
//    {
//        _nowEquipedWeapon.OnUpdate();
//    }

//    void InitializeWeapons()
//    {
//        //IWeaponContainer container = _weaponParent.GetComponentInChildren<IWeaponContainer>();
//        //_nowEquipedWeapon = container.ReturnWeapon();
//        //_nowEquipedWeapon.Initialize(gameObject, _armMesh, _cameraHolder, _ownerAnimator);
//    }
//}
