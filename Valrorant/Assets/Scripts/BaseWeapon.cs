using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 드랍, 무게 등등 다양한 변수 추가
abstract public class BaseWeapon : MonoBehaviour
{
    protected Transform _weaponOwner;

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

    protected Animator _ownerAnimator;

    public Animator OwnerAnimator { get { return _ownerAnimator; } }

    protected bool stopOtherAction = false;

    protected Coroutine stopOtherActionCoroutine;

    [SerializeField]
    protected float equipFinishTime;

    [SerializeField]
    protected float reloadFinishTime;

    WaitForSeconds waitEquipFinishTime;

    private void Update()
    {
        _mainAction.Tick();
        _subAction.Tick();
    }

    protected virtual void Awake()
    {
        _targetLayer = LayerMask.GetMask("PenetrateTarget");
        waitEquipFinishTime = new WaitForSeconds(equipFinishTime);
    }

    public virtual void Initialize(Transform owner, Transform cam, Animator ownerAnimator)
    {
        _weaponOwner = owner;
        _camTransform = cam;

        _ownerAnimator = ownerAnimator;
    }

    protected virtual void OnAttack() { }

    public virtual void OnEquip() 
    {
        gameObject.SetActive(true);
        _ownerAnimator.Play(_weaponName + "Equip");

        stopOtherActionCoroutine = StartCoroutine(EquipFinishCoroutine(waitEquipFinishTime));
    }

    protected IEnumerator EquipFinishCoroutine(WaitForSeconds waitFinishTime)
    {
        stopOtherAction = true;
        yield return waitFinishTime;
        stopOtherAction = false;
    }

    public virtual void OnUnEquip()
    {
        // 제거해줌
        if (stopOtherActionCoroutine != null)
        {
            StopCoroutine(stopOtherActionCoroutine);
            stopOtherActionCoroutine = null;
        }

        gameObject.SetActive(false);
        stopOtherAction = false;
    }

    public virtual void OnReload() { }

    public void OnMainAction() 
    {
        if (stopOtherAction == true) return;
        _mainAction.DoAction();

    }

    public void OnStopMainAction() 
    {
        if (stopOtherAction == true) return;
        _mainAction.StopAction(); 
    }

    public void OnDoSubAction() 
    {
        if (stopOtherAction == true) return;
        _subAction.DoAction();

    }

    public void OnStopSubAction() 
    {
        if (stopOtherAction == true) return;
        _subAction.StopAction(); 
    }

    protected virtual void ChainMainAction()
    {
        _ownerAnimator.Play(_weaponName + "MainAction", -1, 0);
    }

    protected virtual void ChainSubAction()
    {
        _ownerAnimator.Play(_weaponName + "SubAction", -1, 0);
    }
}