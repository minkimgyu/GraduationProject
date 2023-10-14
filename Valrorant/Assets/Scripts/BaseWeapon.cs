using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 드랍, 무게 등등 다양한 변수 추가
abstract public class BaseWeapon : MonoBehaviour
{
    protected Transform _camTransform;

    [SerializeField]
    protected float _range;
    public float Range { get { return _range; } }

    [SerializeField]
    protected string _hitEffectName;

    [SerializeField]
    protected string _weaponName;

    public string WeaponName { get { return _weaponName; } }

    protected int _targetLayer;


    protected ActionStrategy _mainAction;

    protected ActionStrategy _subAction;


    protected ResultStrategy _mainResult;

    protected ResultStrategy _subResult;

    [SerializeField]
    protected float equipFinishTime;
    public float EquipFinishTime { get { return equipFinishTime; } }

    protected Animator _ownerAnimator;
    public Animator OwnerAnimator { get { return _ownerAnimator; } }

    public void OnUpdate()
    {
        _mainAction.Tick();
        _subAction.Tick();
    }

    public virtual void Initialize(Transform cam, Animator ownerAnimator)
    {
        _camTransform = cam;

        _ownerAnimator = ownerAnimator;
        _targetLayer = LayerMask.GetMask("PenetratableTarget", "ParallelProcessingTarget");
    }

    protected virtual void OnAttack() { }

    public virtual void OnEquip() 
    {
        gameObject.SetActive(true);
        _ownerAnimator.Play(_weaponName + "Equip");
    }

    public virtual void OnUnEquip()
    {
        gameObject.SetActive(false);
    }

    public void OnMainActionStart() 
    {
        _mainAction.Start();
    }

    public void OnMainActionProgress()
    {
        _mainAction.Progress();
    }

    public void OnMainActionEnd()
    {
        _mainAction.End();
    }

    public void OnSubActionStart() 
    {
        _subAction.Start();
    }

    public void OnSubActionProgress()
    {
        _subAction.Progress();
    }

    public void OnSubActionEnd() 
    {
        _subAction.End(); 
    }

    public virtual bool CanReload() { return false; }

    public virtual void OnReload() { }

    public virtual void ReloadAmmo() { }

    public virtual float ReturnReloadFinishTime() { return default; }

    public virtual float ReturnReloadStateExitTime() { return default; }

    protected virtual void ChainMainAction() { }

    protected virtual void ChainSubAction() { }

    public virtual void OffScopeModeInstantly() { } 
}