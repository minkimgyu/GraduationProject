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

    public Name WeaponName { get { return _weaponName; } }
    public Type WeaponType { get { return _weaponType; } }

    protected int _targetLayer; // ���� ��� ���̾�

    protected Dictionary<EventType, EventStrategy> _eventStrategies = new Dictionary<EventType, EventStrategy>();
    protected Dictionary<EventType, ActionStrategy> _actionStrategies = new Dictionary<EventType, ActionStrategy>();
    protected Dictionary<EventType, BaseRecoilStrategy> _recoilStrategies = new Dictionary<EventType, BaseRecoilStrategy>();

    protected ReloadStrategy _reloadStrategy;

    protected float _equipFinishTime;
    public float EquipFinishTime { get { return _equipFinishTime; } }

    /// <summary>
    /// ������ �ִϸ��̼��� �����ų �� ȣ��
    /// </summary>
    protected Action<string, int, float> OnPlayWeaponAnimation;

    [SerializeField] float _weaponWeight = 1;
    public float SlowDownRatioByWeaponWeight { get { return 1.0f / _weaponWeight; } }

    protected WeaponEventBlackboard _weaponEventBlackboard;



    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClipDictionary _clips;

    protected void PlaySFX(SoundType type, bool oneShot = false)
    {
        if (_clips.ContainsKey(type) == false) return;

        if(oneShot)
        {
            _audioSource.PlayOneShot(_clips[type]);
        }
        else
        {
            _audioSource.clip = _clips[type];
            _audioSource.Play();
        }
    }

    void ChangeChildLayer(Transform child, int layer)
    {
        child.gameObject.layer = layer;

        for (int i = 0; i < child.childCount; i++)
        {
            Transform childTransform = child.GetChild(i);
            ChangeChildLayer(childTransform, layer);
        }
    }

    public void ChangeWeaponLayer(bool changeLayer)
    {
        int layer;
        if (changeLayer) layer = LayerMask.NameToLayer("Weapons");
        else layer = LayerMask.NameToLayer("Default");

        ChangeChildLayer(transform, layer);
    }

    public virtual void SetDefaultValue()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null) OnPlayWeaponAnimation = animator.Play;

        _targetLayer = LayerMask.GetMask("PenetratableTarget", "ParallelProcessingTarget"); // ���̾� �Ҵ����ش�.
    }

    public virtual void MatchStrategy() { }

    // ���� �� ������ֱ�
    public virtual void Initialize(AutomaticGunData data, RecoilMapData recoilData) { }
    public virtual void Initialize(KnifeData data) { }
    public virtual void Initialize(ClassicData data, RecoilRangeData mainRangeData, RecoilRangeData subRangeData) { }
    public virtual void Initialize(BuckyData data, RecoilRangeData mainRangeData, RecoilRangeData subRangeData) { }
    public virtual void Initialize(JudgeData data, RecoilRangeData mainRangeData, RecoilRangeData subRangeData) { }
    public virtual void Initialize(StingerData data, RecoilMapData mainMapData, RecoilRangeData subRangeData) { }
    public virtual void Initialize(OperatorData data, RecoilRangeData mainRangeData, RecoilRangeData subRangeData) { }
    public virtual void Initialize(GuardianData data, RecoilRangeData mainRangeData) { }

    // ���⿡�� Strategy �߰����ش�.
    //public virtual void AddStrategies(OdinData data, RecoilMapData recoilData) { } // �̰� ���� �� ������ֱ�

    public void OnUpdate() // --> WeaponFSM�� �����������
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
        PlaySFX(SoundType.Equip, true);

        _weaponEventBlackboard.OnPlayOwnerAnimation?.Invoke(_weaponName + "Equip", -1, 0);
        OnPlayWeaponAnimation?.Invoke("Equip", -1, 0);
    }

    public virtual void OnUnEquip()
    {
        foreach (var action in _actionStrategies) action.Value.TurnOffZoomDirectly();
    }


    public virtual void OnRooting(WeaponEventBlackboard blackboard)  // 
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        gameObject.SetActive(false);

        _weaponEventBlackboard = blackboard;
        foreach (var actions in _actionStrategies) actions.Value.LinkEvent(blackboard);
        foreach (var recoils in _recoilStrategies) recoils.Value.LinkEvent(blackboard);
        _reloadStrategy.LinkEvent(blackboard);
    }

    public virtual bool CanDrop() { return false; }

    public virtual void ThrowWeapon(float force) { }

    /// <summary>
    /// ���⿡�� ���� ���� ��, �ʿ� ���� ������ �̺�Ʈ�� �������ش�.
    /// </summary>
    public virtual void OnDrop(WeaponEventBlackboard blackboard)
    {
        foreach (var action in _actionStrategies) action.Value.UnlinkEvent(blackboard);
        foreach (var recoil in _recoilStrategies) recoil.Value.UnlinkEvent(blackboard);
        _reloadStrategy.UnlinkEvent(blackboard);
        transform.SetParent(null);
    }

    public virtual bool IsAmmoEmpty() { return true; }

    public virtual bool CanAutoReload() { return false; }

    // ���ο� �������� �־ �������ֱ�
    public virtual bool CanReload() { return false; }

    // ���ο� �������� �־ �������ֱ�
    public virtual void OnReload(bool isTPS) { }

    public virtual void RefillAmmo() { }

    // ���� �ϴ� ���߿� ���콺 �Է��� ���� ���� ĵ��
    public bool CanCancelReloadAndGoToMainAction()
    {
        return _reloadStrategy.CanCancelReloadingByLeftClick();
    }

    public bool CanCancelReloadAndGoToSubAction()
    {
        return _reloadStrategy.CanCancelReloadingByRightClick();
    }

    public bool IsReloadFinish() // �������� ���� ���
    {
        return _reloadStrategy.IsReloadFinish();
    }

    public void ResetReload() // �������� ����� ���
    {
        _reloadStrategy.OnCancelReload();
    }


    protected virtual bool CanAttack(EventType type) { return _actionStrategies[type].CanExecute(); }


    // WeaponController���� ȣ��Ǵ� �Է� �̺�Ʈ
    public void OnLeftClickStart() => _eventStrategies[EventType.Main].OnMouseClickStart();
    public void OnLeftClickProcess() => _eventStrategies[EventType.Main].OnMouseClickProcess();
    public void OnLeftClickEnd() => _eventStrategies[EventType.Main].OnMouseClickEnd();

    public void OnRightClickStart() => _eventStrategies[EventType.Sub].OnMouseClickStart();
    public void OnRightClickProgress() => _eventStrategies[EventType.Sub].OnMouseClickProcess();
    public void OnRightClickEnd() => _eventStrategies[EventType.Sub].OnMouseClickEnd();

    /// <summary>
    /// ���콺 Ŭ�� �̺�Ʈ�� ���۵� �� ȣ���
    /// </summary>
    protected virtual void OnEventStart(EventType type) { }

    /// <summary>
    /// ���콺 Ŭ�� �̺�Ʈ ���� ��� ȣ���
    /// </summary>
    protected virtual void OnEventUpdate(EventType type) { }

    /// <summary>
    /// ���콺 Ŭ�� �̺�Ʈ�� ������ ��� ȣ���
    /// </summary>
    protected virtual void OnEventEnd(EventType type) { }

    /// <summary>
    /// Action �̺�Ʈ�� ȣ���
    /// </summary>
    protected virtual void OnAction(EventType type) 
    {
        if (CanAttack(type) == false) return;

        _actionStrategies[type].Execute();
        _recoilStrategies[type].Execute();
    }
}