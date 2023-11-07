using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
abstract public class BaseWeapon : BaseRoutine
{
    public enum Name
    {
        AK,
        AR,
        Knife,
        LMG,
        Pistol,
        Shotgun,
        SMG,
        Sniper
    }

    public enum Type
    {
       Main,
       Sub,
       Melee
    }

    protected Transform _camTransform;

    [SerializeField]
    protected float _range;

    [SerializeField]
    protected Name _weaponName;

    [SerializeField]
    protected Type _weaponType;

    public Name WeaponName { get { return _weaponName; } }
    public Type WeaponType { get { return _weaponType; } }

    protected int _targetLayer;


    protected ActionStrategy _mainActionStrategy;

    protected ActionStrategy _subActionStrategy;


    protected ResultStrategy _mainResultStrategy;

    protected ResultStrategy _subResultStrategy;


    protected RecoilStrategy _mainRecoilStrategy;

    protected RecoilStrategy _subRecoilStrategy;

    protected ReloadStrategy _reloadStrategy;

    [SerializeField]
    protected float _equipFinishTime;
    public float EquipFinishTime { get { return _equipFinishTime; } }

    protected Animator _ownerAnimator;
    public Animator OwnerAnimator { get { return _ownerAnimator; } }

    protected Animator _animator;
    public Animator Animator { get { return _animator; } }

    protected GameObject _player;

    public Action<bool, int, int> OnRoundChangeRequested;

    [SerializeField] float _weaponWeight = 1;
    public float SlowDownRatioByWeaponWeight { get { return 1.0f /_weaponWeight; } }

    protected override void OnUpdate() // --> WeaponFSM�� �����������
    {
        _mainActionStrategy.OnUpdate();
        _subActionStrategy.OnUpdate();

        _mainResultStrategy.OnUpdate();
        _subResultStrategy.OnUpdate();

        _mainRecoilStrategy.OnUpdate();
        _subRecoilStrategy.OnUpdate();

        _reloadStrategy.OnUpdate();
    }

    protected override void OnDisableGameObject() { }

    //////////////////////////////////////////////////////////////////////////////////////////// �̺�Ʈ ����

    void LinkRoundShower()
    {
        GameObject gameObject = FindWithTag("BulletLeftShower");
        LeftRoundShower _leftRoundShower = gameObject.GetComponent<LeftRoundShower>();
        if (_leftRoundShower == null) return;

        OnRoundChangeRequested += _leftRoundShower.OnRoundCountChange;
    }

    void UnLinkRoundShower()
    {
        GameObject gameObject = FindWithTag("BulletLeftShower");
        LeftRoundShower _leftRoundShower = gameObject.GetComponent<LeftRoundShower>();
        if (_leftRoundShower == null) return;

        OnRoundChangeRequested -= _leftRoundShower.OnRoundCountChange;
    }

    /// <summary>
    /// ���Ⱑ �����Ǹ� Initialize ���� 1�� �����
    /// </summary>
    public virtual void Initialize(GameObject player, GameObject armMesh, Transform cam, Animator ownerAnimator)
    {
        _player = player;
        _camTransform = cam;
        _ownerAnimator = ownerAnimator;
        _animator = GetComponent<Animator>();

        _targetLayer = LayerMask.GetMask("PenetratableTarget", "ParallelProcessingTarget");
        LinkRoundShower();
    }

    /// <summary>
    /// ���⿡�� ���� ���� ��, �ʿ� ���� ������ �̺�Ʈ�� �������ش�.
    /// </summary>
    public virtual void OnDrop(GameObject player)
    {
        _player = null;
        _camTransform = null;
        _ownerAnimator = null;

        UnLinkRoundShower();

        _mainRecoilStrategy.OnUnlink(player);
        _mainResultStrategy.OnUnLink(player);

        _subRecoilStrategy.OnUnlink(player);
        _subResultStrategy.OnUnLink(player);

        _reloadStrategy.OnUnlink();
    }

    /// <summary>
    /// ���� ���� �� �ʱ�ȭ ����. ���� ���⸦ �ֿ��� �� ����
    /// </summary>
    public virtual void OnRooting(GameObject player, Transform cam, Animator ownerAnimator)  // 
    {
        _player = player;
        _camTransform = cam;
        _ownerAnimator = ownerAnimator;

        LinkRoundShower();

        _mainRecoilStrategy.OnLink(player);
        _mainResultStrategy.OnUnLink(player);

        _subRecoilStrategy.OnLink(player);
        _subResultStrategy.OnUnLink(player);

        _reloadStrategy.OnLink();
    }

    /// ����, ���� �̺�Ʈ
    ////////////////////////////////////////////////////////////////////////////////////

    public virtual void OnEquip()
    {
        gameObject.SetActive(true);
        _ownerAnimator.Play(_weaponName + "Equip");
        _animator.Play("Equip");
    }

    public virtual void OnUnEquip()
    {
        _mainResultStrategy.OnReload();
        _subResultStrategy.OnReload();
        gameObject.SetActive(false);
    }

    /// AutoReload �̺�Ʈ
    ////////////////////////////////////////////////////////////////////////////////////

    public virtual bool CanAutoReload() { return false; }

    public virtual bool IsMagazineEmptyWhenProcessingAction() { return false; } // �׼� ���߿� ź�� ����� ���

    /// ReloadStrategy �̺�Ʈ
    ////////////////////////////////////////////////////////////////////////////////////

    
    public bool IsReloadFinish() // �������� ���� ���
    {
        return _reloadStrategy.IsReloadFinish();
    }

    public void ResetReload() // �������� ����� ���
    {
        _reloadStrategy.OnResetReload();
    }

    /// Reload �̺�Ʈ --> ReloadStrategy ������ �Ѿ� ��� ���� �Լ�
    ////////////////////////////////////////////////////////////////////////////////////

    public bool CancelReloadAndGoToMainAction() 
    {
        return _reloadStrategy.CancelReloadAndGoToMainAction(); 
    }

    public bool CancelReloadAndGoToSubAction()
    {
        return _reloadStrategy.CancelReloadAndGoToSubAction();
    }

    public virtual bool CanReload() { return false; }
    public virtual void OnReload() { }

    /// ActionStrategy �̺�Ʈ
    ////////////////////////////////////////////////////////////////////////////////////

    protected virtual void OnMainActionEventCallRequsted()
    {
        if (_mainResultStrategy.CanDo() == false) return; // ResultStrategy���� ���� �����ϴٸ�

        _mainRecoilStrategy.OnEventRequested();
        _subRecoilStrategy.OnOtherActionEventRequested();

        _mainResultStrategy.Do(); // --> ���⼭ ��밡������ üũ
        _subResultStrategy.OnOtherActionEventRequested(); // Į ��Ÿ�� ����
    }

    protected virtual void OnSubActionEventCallRequsted()
    {
        if (_subResultStrategy.CanDo() == false) return; // ResultStrategy���� ���� �����ϴٸ�

        _subRecoilStrategy.OnEventRequested();
        _mainRecoilStrategy.OnOtherActionEventRequested();

        _subResultStrategy.Do();
        _mainResultStrategy.OnOtherActionEventRequested(); // Į ��Ÿ�� ����
    }

    protected virtual void OnMainActionEventCallFinished()
    {
        _mainRecoilStrategy.OnEventFinished();
    }

    protected virtual void OnSubActionEventCallFinished()
    {
        _subRecoilStrategy.OnEventFinished();
    }

    protected virtual void OnMainActionClickStart() { }

    protected virtual void OnMainActionClickProgress() { }

    protected virtual void OnMainActionClickEnd() 
    {
        _mainRecoilStrategy.OnClickEnd();
    }

    protected virtual void OnSubActionClickStart() { }

    protected virtual void OnSubActionClickProgress() { }

    protected virtual void OnSubActionClickEnd() 
    {
        _subRecoilStrategy.OnClickEnd();
    }

    /// �̺�Ʈ ����
    //////////////////////////////////////////////////////////////////////////////////////////////////////

    public void OnLeftClickStart()
    {
        _mainActionStrategy.OnMouseClickStart();
    }

    public void OnLeftClickProgress()
    {
        _mainActionStrategy.OnMouseClickProgress();
    }

    public void OnLeftClickEnd()
    {
        _mainActionStrategy.OnMouseClickEnd();
    }

    public void OnRightClickStart()
    {
        _subActionStrategy.OnMouseClickStart();
    }

    public void OnRightClickProgress()
    {
        _subActionStrategy.OnMouseClickProgress();
    }

    public void OnRightClickEnd()
    {
        _subActionStrategy.OnMouseClickEnd();
    }

    protected virtual void LinkEvent(GameObject player)
    {

    }

    protected void LinkActionEvent(bool isMain, bool nowLink)
    {
        if(isMain)
        {
            if(nowLink)
            {
                _mainActionStrategy.OnActionStart += OnMainActionClickStart;
                _mainActionStrategy.OnActionProgress += OnMainActionClickProgress;
                _mainActionStrategy.OnActionEnd += OnMainActionClickEnd;
                _mainActionStrategy.OnEventCallRequsted += OnMainActionEventCallRequsted;
                _mainActionStrategy.OnEventCallFinished += OnMainActionEventCallFinished;
            }
            else
            {
                _mainActionStrategy.OnActionStart -= OnMainActionClickStart;
                _mainActionStrategy.OnActionProgress -= OnMainActionClickProgress;
                _mainActionStrategy.OnActionEnd -= OnMainActionClickEnd;
                _mainActionStrategy.OnEventCallRequsted -= OnMainActionEventCallRequsted;
                _mainActionStrategy.OnEventCallFinished -= OnMainActionEventCallFinished;
            }
        }
        else
        {
            if (nowLink)
            {
                _subActionStrategy.OnActionStart += OnSubActionClickStart;
                _subActionStrategy.OnActionProgress += OnSubActionClickProgress;
                _subActionStrategy.OnActionEnd += OnSubActionClickEnd;
                _subActionStrategy.OnEventCallRequsted += OnSubActionEventCallRequsted;
                _subActionStrategy.OnEventCallFinished += OnSubActionEventCallFinished;
            }
            else
            {
                _subActionStrategy.OnActionStart -= OnSubActionClickStart;
                _subActionStrategy.OnActionProgress -= OnSubActionClickProgress;
                _subActionStrategy.OnActionEnd -= OnSubActionClickEnd;
                _subActionStrategy.OnEventCallRequsted -= OnSubActionEventCallRequsted;
                _subActionStrategy.OnEventCallFinished -= OnSubActionEventCallFinished;
            }
        }
    }
}

//_mainActionStrategy.OnActionStart = OnMainActionClickStart;
//_mainActionStrategy.OnActionProgress = OnMainActionClickProgress;
//_mainActionStrategy.OnActionEnd = OnMainActionClickEnd;

//_mainActionStrategy.OnEventCallRequsted = OnMainActionEventCallRequsted;
//_subActionStrategy.OnEventCallRequsted = OnSubActionEventCallRequsted;

//_subActionStrategy.OnActionStart = OnSubActionClickStart;
//_subActionStrategy.OnActionProgress = OnSubActionClickProgress;
//_subActionStrategy.OnActionEnd = OnSubActionClickEnd;

//_mainActionStrategy.OnEventCallFinished = OnMainActionEventCallFinished;
//_subActionStrategy.OnEventCallFinished = OnSubActionEventCallFinished;

//_mainResultStrategy.OnInintialize(player);
//_subResultStrategy.OnInintialize(player);

//_mainRecoilStrategy.OnInintialize(player);
//_subRecoilStrategy.OnInintialize(player);

//_reloadStrategy.OnInintialize();