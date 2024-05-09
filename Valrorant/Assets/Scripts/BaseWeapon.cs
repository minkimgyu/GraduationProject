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
        AutoShotgun,
        SMG,
        Sniper,
        DMR
    }

    public enum Type
    {
        None,
        Main,
        Sub,
        Melee
    }

    public enum EventType
    {
        Main,
        Sub
    }

    protected float _range;

    [SerializeField] protected Name _weaponName;

    [SerializeField] protected Type _weaponType;

    public Type WeaponType { get { return _weaponType; } }

    protected int _targetLayer; // 공격 대상 레이어

    protected Dictionary<EventType, EventStrategy> _eventStrategies = new Dictionary<EventType, EventStrategy>();
    protected Dictionary<EventType, ActionStrategy> _actionStrategies = new Dictionary<EventType, ActionStrategy>();
    protected Dictionary<EventType, BaseRecoilStrategy> _recoilStrategies = new Dictionary<EventType, BaseRecoilStrategy>();

    protected ReloadStrategy _reloadStrategy;


    protected float _equipFinishTime;
    public float EquipFinishTime { get { return _equipFinishTime; } }

    /// <summary>
    /// 무기의 애니메이션을 실행시킬 때 호출
    /// </summary>
    protected Action<string, int, float> OnPlayWeaponAnimation;

    public Action<bool, int, int> OnRoundChangeRequested;

    [SerializeField] float _weaponWeight = 1;
    public float SlowDownRatioByWeaponWeight { get { return 1.0f / _weaponWeight; } }

    protected WeaponEventBlackboard _weaponEventBlackboard;


    public virtual void SetDefaultValue()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null) OnPlayWeaponAnimation = animator.Play;

        _targetLayer = LayerMask.GetMask("PenetratableTarget", "ParallelProcessingTarget"); // 레이어 할당해준다.
    }
    public virtual void MatchStrategy() { }

    // 여러 개 만들어주기
    public virtual void Initialize(AutomaticGunData data, RecoilMapData recoilData) { }
    public virtual void Initialize(KnifeData data) { }
    public virtual void Initialize(ClassicData data, RecoilRangeData mainRangeData, RecoilRangeData subRangeData) { }
    public virtual void Initialize(BuckyData data, RecoilRangeData mainRangeData, RecoilRangeData subRangeData) { }
    public virtual void Initialize(JudgeData data, RecoilRangeData mainRangeData, RecoilRangeData subRangeData) { }
    public virtual void Initialize(StingerData data, RecoilMapData mainMapData, RecoilRangeData subRangeData) { }
    public virtual void Initialize(OperatorData data, RecoilRangeData mainRangeData, RecoilRangeData subRangeData) { }
    public virtual void Initialize(GuardianData data, RecoilRangeData mainRangeData) { }

    // 여기에서 Strategy 추가해준다.
    //public virtual void AddStrategies(OdinData data, RecoilMapData recoilData) { } // 이걸 여러 개 만들어주기

    public void OnUpdate() // --> WeaponFSM에 연결시켜주자
    {
        foreach (var events in _eventStrategies) events.Value.OnUpdate();
        foreach (var actions in _actionStrategies) actions.Value.OnUpdate();
        foreach (var recoils in _recoilStrategies) recoils.Value.OnUpdate();

        _reloadStrategy.OnUpdate();
    }

    protected virtual void OnCollisionEnter(Collision collision) { }

    public virtual void PositionWeapon(bool nowDrop) { }

    public virtual void OnEquip()
    {
        gameObject.SetActive(true);
        _weaponEventBlackboard.OnPlayOwnerAnimation?.Invoke(_weaponName + "Equip", -1, 0);
        OnPlayWeaponAnimation?.Invoke("Equip", -1, 0);
    }

    public virtual void OnUnEquip()
    {
        foreach (var action in _actionStrategies) action.Value.TurnOffZoomDirectly();
        gameObject.SetActive(false);
    }


    public virtual void OnRooting(WeaponEventBlackboard blackboard)  // 
    {
        _weaponEventBlackboard = blackboard;
        foreach (var actions in _actionStrategies) actions.Value.LinkEvent(blackboard);
        foreach (var recoils in _recoilStrategies) recoils.Value.LinkEvent(blackboard);
        _reloadStrategy.LinkEvent(blackboard);
    }

    public virtual bool CanDrop() { return false; }

    public virtual void ThrowWeapon(float force) { }

    /// <summary>
    /// 여기에서 무기 해제 시, 필요 없는 변수나 이벤트를 해제해준다.
    /// </summary>
    public virtual void OnDrop(WeaponEventBlackboard blackboard)
    {
        foreach (var action in _actionStrategies) action.Value.UnlinkEvent(blackboard);
        foreach (var recoil in _recoilStrategies) recoil.Value.UnlinkEvent(blackboard);
        _reloadStrategy.UnlinkEvent(blackboard);
        transform.SetParent(null);
    }


    public virtual bool CanAutoReload() { return false; }

    // 내부에 전략패턴 넣어서 구현해주기
    public virtual bool CanReload() { return false; }

    // 내부에 전략패턴 넣어서 구현해주기
    public virtual void OnReload(bool isTPS) { }


    // 장전 하는 도중에 마우스 입력을 통한 장전 캔슬
    public bool CanCancelReloadAndGoToMainAction()
    {
        return _reloadStrategy.CanCancelReloadingByLeftClick();
    }

    public bool CanCancelReloadAndGoToSubAction()
    {
        return _reloadStrategy.CanCancelReloadingByRightClick();
    }

    public bool IsReloadFinish() // 재장전이 끝난 경우
    {
        return _reloadStrategy.IsReloadFinish();
    }

    public void ResetReload() // 재장전을 취소할 경우
    {
        _reloadStrategy.OnCancelReload();
    }


    protected virtual bool CanAttack(EventType type) { return _actionStrategies[type].CanExecute(); }


    // WeaponController에서 호출되는 입력 이벤트
    public void OnLeftClickStart() => _eventStrategies[EventType.Main].OnMouseClickStart();
    public void OnLeftClickProcess() => _eventStrategies[EventType.Main].OnMouseClickProcess();
    public void OnLeftClickEnd() => _eventStrategies[EventType.Main].OnMouseClickEnd();

    public void OnRightClickStart() => _eventStrategies[EventType.Sub].OnMouseClickStart();
    public void OnRightClickProgress() => _eventStrategies[EventType.Sub].OnMouseClickProcess();
    public void OnRightClickEnd() => _eventStrategies[EventType.Sub].OnMouseClickEnd();

    /// <summary>
    /// 마우스 클릭 이벤트가 시작될 때 호출됨
    /// </summary>
    protected virtual void OnEventStart(EventType type) { }

    /// <summary>
    /// 마우스 클릭 이벤트 중인 경우 호출됨
    /// </summary>
    protected virtual void OnEventUpdate(EventType type) { }

    /// <summary>
    /// 마우스 클릭 이벤트가 끝나는 경우 호출됨
    /// </summary>
    protected virtual void OnEventEnd(EventType type) { }

    /// <summary>
    /// Action 이벤트가 호출됨
    /// </summary>
    protected virtual void OnAction(EventType type) 
    {
        if (CanAttack(type) == false) return;

        _actionStrategies[type].Execute();
        _recoilStrategies[type].Execute();
    }
}