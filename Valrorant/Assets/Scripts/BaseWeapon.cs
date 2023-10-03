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


    protected ResultStrategy _mainResult;

    protected ResultStrategy _subResult;



    protected Animator _ownerAnimator;

    public Animator OwnerAnimator { get { return _ownerAnimator; } }

    protected bool stopOtherAction = false;

    protected Coroutine stopOtherActionCoroutine;

    [SerializeField]
    protected float equipFinishTime;

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

        _subResult.UnEquip();
    }

    public virtual void OnReload() { }



    public void OnMainActionStart() 
    {
        if (stopOtherAction == true) return;
        _mainAction.Start();
    }

    public void OnMainActionProgress()
    {
        if (stopOtherAction == true) return;
        _mainAction.Progress();
    }

    public void OnMainActionEnd()
    {
        if (stopOtherAction == true) return;
        _mainAction.End();
    }



    public void OnSubActionStart() 
    {
        if (stopOtherAction == true) return;
        _subAction.Start();

    }

    public void OnSubActionProgress()
    {
        if (stopOtherAction == true) return;
        _subAction.Progress();
    }

    public void OnSubActionEnd() 
    {
        if (stopOtherAction == true) return;
        _subAction.End(); 
    }


    protected virtual void ChainMainAction() { }

    protected virtual void ChainSubAction() { }
}