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


    protected int _targetLayer; // 공격 대상 레이어


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
    /// 무기를 보유한 대상의 애니메이터
    /// </summary>
    protected Animator _ownerAnimator; 
    public Animator OwnerAnimator { get { return _ownerAnimator; } }

    /// <summary>
    /// 무기의 에니메이터
    /// </summary>
    protected Animator _animator;
    public Animator Animator { get { return _animator; } }

    /// <summary>
    /// 무기의 애니메이션을 실행시킬 때 호출
    /// </summary>
    Action<string, int, float> OnPlayWeaponAnimation;

    /// <summary>
    /// 무기를 소유한 대상의 애니메이션을 실행시킬 때 호출
    /// </summary>
    Action<string, int, float> OnPlayOwnerAnimation;


    protected GameObject _player;

    public Action<bool, int, int> OnRoundChangeRequested;

    [SerializeField] float _weaponWeight = 1;
    public float SlowDownRatioByWeaponWeight { get { return 1.0f /_weaponWeight; } }

    public void RunUpdateInController() // --> WeaponFSM에 연결시켜주자
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

    //public virtual void RefillAmmo() {  } --> 이거는 ReloadStrategy를 보고 참고해야할 듯

    //public virtual bool NowNeedToRefillAmmo() { return false; }

    //////////////////////////////////////////////////////////////////////////////////////////// 이벤트 모음

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
    /// 무기가 생성되면 Initialize 최초 1번 실행됨
    /// </summary>
    public virtual void Initialize(GameObject player, GameObject armMesh, Transform cam, Animator ownerAnimator)
    {
        _player = player;
        _camTransform = cam;
        _ownerAnimator = ownerAnimator;
        _animator = GetComponent<Animator>();

        OnPlayOwnerAnimation += _ownerAnimator.Play; // 무기 버릴 때 빼줘야함
        OnPlayWeaponAnimation = _animator.Play;

        _targetLayer = LayerMask.GetMask("PenetratableTarget", "ParallelProcessingTarget");

        if (player.tag != "Player") return;
        LinkRoundViewer(true);
    }

    /// <summary>
    /// 여기에서 무기 해제 시, 필요 없는 변수나 이벤트를 해제해준다.
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
    /// 무기 장착 시 초기화 진행. 버린 무기를 주웠을 때 실행
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

    /// 장착, 해제 이벤트
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

    public bool IsReloadRunning() // 재장전 중인 경우
    {
        return _reloadStrategy.IsReloadRunning();
    }

    public void ResetReload() // 재장전을 취소할 경우
    {
        _reloadStrategy.OnResetReload();
        // 애니메이션 idle 초기화

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

    //public virtual bool NeedToReload() { return true; }

    public virtual bool CanReload() { return false; }

    public virtual bool CanAttack() { return false; }

    public virtual void OnReload() { }

    /// ActionStrategy 이벤트
    ////////////////////////////////////////////////////////////////////////////////////

    protected virtual void OnMainEventCallRequsted()
    {
        if (_mainActionStrategy.CanAct() == false) return; // ResultStrategy에서 진행 가능하다면

        _mainRecoilStrategy.OnEventRequested();
        _subRecoilStrategy.OnOtherActionEventRequested();

        _mainActionStrategy.Do(); // --> 여기서 사용가능한지 체크
        _subActionStrategy.OnOtherActionEventRequested(); // 칼 쿨타임 적용
    }

    protected virtual void OnSubEventCallRequsted()
    {
        if (_subActionStrategy.CanAct() == false) return; // ResultStrategy에서 진행 가능하다면

        _subRecoilStrategy.OnEventRequested();
        _mainRecoilStrategy.OnOtherActionEventRequested();

        _subActionStrategy.Do();
        _mainActionStrategy.OnOtherActionEventRequested(); // 칼 쿨타임 적용
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

    /// 이벤트 연결
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