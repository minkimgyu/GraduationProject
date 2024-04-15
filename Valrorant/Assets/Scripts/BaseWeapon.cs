using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
abstract public class BaseWeapon : MonoBehaviour
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
       None,
       Main,
       Sub,
       Melee
    }

    public enum StrategyType
    {
        Main,
        Sub
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


    protected int _targetLayer; // ���� ��� ���̾�


    protected Dictionary<StrategyType, EventStrategy> _eventStrategies;
    protected Dictionary<StrategyType, ActionStrategy> _actionStrategies;
    protected Dictionary<StrategyType, RecoilStrategy> _recoilStrategies;

    protected ReloadStrategy _reloadStrategy;

    protected EventStrategy _mainEventStrategy;

    protected EventStrategy _subEventStrategy;


    protected ActionStrategy _mainActionStrategy;

    protected ActionStrategy _subActionStrategy;


    protected RecoilStrategy _mainRecoilStrategy;

    protected RecoilStrategy _subRecoilStrategy;


    //protected ReloadStrategy _reloadStrategy;

    [SerializeField]
    protected float _equipFinishTime;
    public float EquipFinishTime { get { return _equipFinishTime; } }

    /// <summary>
    /// ���⸦ ������ ����� �ִϸ�����
    /// </summary>
    protected Animator _ownerAnimator; 
    public Animator OwnerAnimator { get { return _ownerAnimator; } }

    /// <summary>
    /// ������ ���ϸ�����
    /// </summary>
    protected Animator _animator;
    public Animator Animator { get { return _animator; } }

    /// <summary>
    /// ������ �ִϸ��̼��� �����ų �� ȣ��
    /// </summary>
    Action<string, int, float> OnPlayWeaponAnimation;

    /// <summary>
    /// ���⸦ ������ ����� �ִϸ��̼��� �����ų �� ȣ��
    /// </summary>
    Action<string, int, float> OnPlayOwnerAnimation;


    protected GameObject _player;

    public Action<bool, int, int> OnRoundChangeRequested;

    [SerializeField] float _weaponWeight = 1;
    public float SlowDownRatioByWeaponWeight { get { return 1.0f /_weaponWeight; } }

    public void RunUpdateInController() // --> WeaponFSM�� �����������
    {
        _eventStrategies[StrategyType.Main].OnUpdate();
        _eventStrategies[StrategyType.Sub].OnUpdate();

        _actionStrategies[StrategyType.Main].OnUpdate();
        _actionStrategies[StrategyType.Sub].OnUpdate();

        _recoilStrategies[StrategyType.Main].OnUpdate();
        _recoilStrategies[StrategyType.Sub].OnUpdate();

        _reloadStrategy.OnUpdate();

        //_mainEventStrategy.OnUpdate();
        //_subEventStrategy.OnUpdate();

        //_mainActionStrategy.OnUpdate();
        //_subActionStrategy.OnUpdate();

        //_mainRecoilStrategy.OnUpdate();
        //_subRecoilStrategy.OnUpdate();

        //_reloadStrategy.OnUpdate();
    }

    public virtual bool CanDrop() { return false; }

    protected virtual void OnCollisionEnter(Collision collision) { }

    public virtual void Drop(float force) { }

    //public virtual void RefillAmmo() {  } --> �̰Ŵ� ReloadStrategy�� ���� �����ؾ��� ��

    //public virtual bool NowNeedToRefillAmmo() { return false; }

    //////////////////////////////////////////////////////////////////////////////////////////// �̺�Ʈ ����

    void LinkRoundViewer(bool nowLink)
    {
        GameObject gameObject = GameObject.FindWithTag("BulletLeftShower");
        if (gameObject == null) return;

        LeftRoundShower _leftRoundShower = gameObject.GetComponent<LeftRoundShower>();
        if (_leftRoundShower == null) return;

        if (nowLink) OnRoundChangeRequested += _leftRoundShower.OnRoundCountChange;
        else OnRoundChangeRequested -= _leftRoundShower.OnRoundCountChange;
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

        OnPlayOwnerAnimation += _ownerAnimator.Play; // ���� ���� �� �������
        OnPlayWeaponAnimation = _animator.Play;

        _targetLayer = LayerMask.GetMask("PenetratableTarget", "ParallelProcessingTarget");

        if (player.tag != "Player") return;
        LinkRoundViewer(true);
    }

    /// <summary>
    /// ���⿡�� ���� ���� ��, �ʿ� ���� ������ �̺�Ʈ�� �������ش�.
    /// </summary>
    public virtual void OnDrop()
    {
        LinkRoundViewer(false);

        _mainRecoilStrategy.OnUnlink(_player);
        _mainActionStrategy.TurnOffAim();

        _mainActionStrategy.OnUnLink(_player);

        _subActionStrategy.TurnOffAim();

        _subActionStrategy.OnUnLink(_player);
        _subActionStrategy.TurnOffAim();

        _subRecoilStrategy.OnUnlink(_player);

        _player = null;
        _camTransform = null;
        _ownerAnimator = null;

        _animator.Play("Idle", -1, 0);

        transform.SetParent(null);
    }

    /// <summary>
    /// ���� ���� �� �ʱ�ȭ ����. ���� ���⸦ �ֿ��� �� ����
    /// </summary>
    public virtual void OnRooting(GameObject player, Transform cam, Animator ownerAnimator)  // 
    {
        _player = player;
        _camTransform = cam;
        _ownerAnimator = ownerAnimator;

        LinkRoundViewer(true);

        _mainRecoilStrategy.OnLink(player);
        _mainActionStrategy.OnLink(player);

        _subRecoilStrategy.OnLink(player);
        _subActionStrategy.OnLink(player);
    }

    public virtual void PositionWeaponMesh(bool nowDrop) { }

    /// ����, ���� �̺�Ʈ
    ////////////////////////////////////////////////////////////////////////////////////

    public virtual void OnEquip()
    {
        gameObject.SetActive(true);
        _ownerAnimator.Play(_weaponName + "Equip", -1, 0);
        _animator.Play("Equip", -1, 0);
    }

    public virtual void OnUnEquip()
    {
        _mainActionStrategy.OnReload();
        _subActionStrategy.OnReload();
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

    public bool IsReloadRunning() // ������ ���� ���
    {
        return _reloadStrategy.IsReloadRunning();
    }

    public void ResetReload() // �������� ����� ���
    {
        _reloadStrategy.OnResetReload();
        // �ִϸ��̼� idle �ʱ�ȭ

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

    //public virtual bool NeedToReload() { return true; }

    public virtual bool CanReload() { return false; }

    public virtual bool CanAttack() { return false; }

    public virtual void OnReload() { }

    /// ActionStrategy �̺�Ʈ
    ////////////////////////////////////////////////////////////////////////////////////

    protected virtual void OnMainEventCallRequsted()
    {
        if (_mainActionStrategy.CanAct() == false) return; // ResultStrategy���� ���� �����ϴٸ�

        _mainRecoilStrategy.OnEventRequested();
        _subRecoilStrategy.OnOtherActionEventRequested();

        _mainActionStrategy.Do(); // --> ���⼭ ��밡������ üũ
        _subActionStrategy.OnOtherActionEventRequested(); // Į ��Ÿ�� ����
    }

    protected virtual void OnSubEventCallRequsted()
    {
        if (_subActionStrategy.CanAct() == false) return; // ResultStrategy���� ���� �����ϴٸ�

        _subRecoilStrategy.OnEventRequested();
        _mainRecoilStrategy.OnOtherActionEventRequested();

        _subActionStrategy.Do();
        _mainActionStrategy.OnOtherActionEventRequested(); // Į ��Ÿ�� ����
    }

    protected virtual void OnMainEventFinished()
    {
        _mainRecoilStrategy.OnEventFinished();
    }

    protected virtual void OnSubEventFinished()
    {
        _subRecoilStrategy.OnEventFinished();
    }

    protected virtual void OnMainEventStart() { }

    protected virtual void OnMainEventProgress() { }

    protected virtual void OnMainEventEnd() 
    {
        _mainRecoilStrategy.OnClickEnd();
    }

    protected virtual void OnSubEventStart() { }

    protected virtual void OnSubEventProgress() { }

    protected virtual void OnSubEventEnd() 
    {
        _subRecoilStrategy.OnClickEnd();
    }

    /// �̺�Ʈ ����
    //////////////////////////////////////////////////////////////////////////////////////////////////////

    public void OnLeftClickStart()
    {
        _mainEventStrategy.OnMouseClickStart();
    }

    public void OnLeftClickProgress()
    {
        _mainEventStrategy.OnMouseClickProgress();
    }

    public void OnLeftClickEnd()
    {
        _mainEventStrategy.OnMouseClickEnd();
    }

    public void OnRightClickStart()
    {
        _subEventStrategy.OnMouseClickStart();
    }

    public void OnRightClickProgress()
    {
        _subEventStrategy.OnMouseClickProgress();
    }

    public void OnRightClickEnd()
    {
        _subEventStrategy.OnMouseClickEnd();
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
                _mainEventStrategy.OnActionStart += OnMainEventStart;
                _mainEventStrategy.OnActionProgress += OnMainEventProgress;
                _mainEventStrategy.OnActionEnd += OnMainEventEnd;
                _mainEventStrategy.OnEventCallRequsted += OnMainEventCallRequsted;
                _mainEventStrategy.OnEventCallFinished += OnMainEventFinished;
            }
            else
            {
                _mainEventStrategy.OnActionStart -= OnMainEventStart;
                _mainEventStrategy.OnActionProgress -= OnMainEventProgress;
                _mainEventStrategy.OnActionEnd -= OnMainEventEnd;
                _mainEventStrategy.OnEventCallRequsted -= OnMainEventCallRequsted;
                _mainEventStrategy.OnEventCallFinished -= OnMainEventFinished;
            }
        }
        else
        {
            if (nowLink)
            {
                _subEventStrategy.OnActionStart += OnSubEventStart;
                _subEventStrategy.OnActionProgress += OnSubEventProgress;
                _subEventStrategy.OnActionEnd += OnSubEventEnd;

                _subEventStrategy.OnEventCallRequsted += OnSubEventCallRequsted;
                _subEventStrategy.OnEventCallFinished += OnSubEventFinished;
            }
            else
            {
                _subEventStrategy.OnActionStart -= OnSubEventStart;
                _subEventStrategy.OnActionProgress -= OnSubEventProgress;
                _subEventStrategy.OnActionEnd -= OnSubEventEnd;
                _subEventStrategy.OnEventCallRequsted -= OnSubEventCallRequsted;
                _subEventStrategy.OnEventCallFinished -= OnSubEventFinished;
            }
        }
    }
}