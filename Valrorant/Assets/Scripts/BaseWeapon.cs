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

    protected override void OnUpdate() // --> WeaponFSM에 연결시켜주자
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

    //////////////////////////////////////////////////////////////////////////////////////////// 이벤트 모음

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
    /// 무기가 생성되면 Initialize 최초 1번 실행됨
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
    /// 여기에서 무기 해제 시, 필요 없는 변수나 이벤트를 해제해준다.
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
    /// 무기 장착 시 초기화 진행. 버린 무기를 주웠을 때 실행
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

    /// 장착, 해제 이벤트
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

    /// AutoReload 이벤트
    ////////////////////////////////////////////////////////////////////////////////////

    public virtual bool CanAutoReload() { return false; }

    public virtual bool IsMagazineEmptyWhenProcessingAction() { return false; } // 액션 도중에 탄이 비워진 경우

    /// ReloadStrategy 이벤트
    ////////////////////////////////////////////////////////////////////////////////////

    
    public bool IsReloadFinish() // 재장전이 끝난 경우
    {
        return _reloadStrategy.IsReloadFinish();
    }

    public void ResetReload() // 재장전을 취소할 경우
    {
        _reloadStrategy.OnResetReload();
    }

    /// Reload 이벤트 --> ReloadStrategy 제외한 총알 계산 관련 함수
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

    /// ActionStrategy 이벤트
    ////////////////////////////////////////////////////////////////////////////////////

    protected virtual void OnMainActionEventCallRequsted()
    {
        if (_mainResultStrategy.CanDo() == false) return; // ResultStrategy에서 진행 가능하다면

        _mainRecoilStrategy.OnEventRequested();
        _subRecoilStrategy.OnOtherActionEventRequested();

        _mainResultStrategy.Do(); // --> 여기서 사용가능한지 체크
        _subResultStrategy.OnOtherActionEventRequested(); // 칼 쿨타임 적용
    }

    protected virtual void OnSubActionEventCallRequsted()
    {
        if (_subResultStrategy.CanDo() == false) return; // ResultStrategy에서 진행 가능하다면

        _subRecoilStrategy.OnEventRequested();
        _mainRecoilStrategy.OnOtherActionEventRequested();

        _subResultStrategy.Do();
        _mainResultStrategy.OnOtherActionEventRequested(); // 칼 쿨타임 적용
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

    /// 이벤트 연결
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