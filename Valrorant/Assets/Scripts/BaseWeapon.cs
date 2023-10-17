using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObserverPattern;

// ���, ���� ��� �پ��� ���� �߰�
abstract public class BaseWeapon : MonoBehaviour, ISubject<int, int>
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


    protected RecoilStrategy _mainRecoilGenerator; // noRecoil ��� --> ���� �������� ó��

    protected RecoilStrategy _subRecoilGenerator;


    [SerializeField]
    protected float equipFinishTime;
    public float EquipFinishTime { get { return equipFinishTime; } }

    protected Animator _ownerAnimator;
    public Animator OwnerAnimator { get { return _ownerAnimator; } }

    protected GameObject _player;
    public List<IObserver<int, int>> Observers { get; set; }


    public void AddObserver(IObserver<int, int> observer)
    {
        Observers.Add(observer);
    }

    public void RemoveObserver(IObserver<int, int> observer)
    {
        Observers.Remove(observer);
    }

    public void NotifyToObservers(int inMagazine = 0, int inPossession = 0)
    {
        for (int i = 0; i < Observers.Count; i++)
        {
            Observers[i].Notify(inMagazine, inPossession);
        }
    }

    protected virtual void Update()
    {
        _mainAction.OnUpdate();
        _subAction.OnUpdate();

        _mainResult.OnUpdate();
        _subResult.OnUpdate();

        _mainRecoilGenerator.OnUpdate();
        _subRecoilGenerator.OnUpdate();
    }

    public virtual void Initialize(GameObject player, Transform cam, Animator ownerAnimator)
    {
        _player = player;

        _camTransform = cam;

        _ownerAnimator = ownerAnimator;
        _targetLayer = LayerMask.GetMask("PenetratableTarget", "ParallelProcessingTarget");

        Observers = new List<IObserver<int, int>>();
    }

    protected void LinkActionStrategy()
    {
        _mainAction.OnActionStart = ChainMainActionStartEvent;
        _mainAction.OnActionProgress = ChainMainActionProgressEvent;
        _mainAction.OnActionEnd = ChainMainActionEndEvent;

        _subAction.OnActionStart = ChainSubActionStartEvent;
        _subAction.OnActionProgress = ChainSubActionProgressEvent;
        _subAction.OnActionEnd = ChainSubActionEndEvent;
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

    // ���������� ������
    // --> �Ѿ��� �� �������� ���
    // --> Ư�� ��Ȳ���� ������ ���� ���ϴ� ���

    public void StartMainAction()
    {
        _mainAction.OnMouseClickStart();
    }

    public void ProgressMainAction()
    {
        _mainAction.OnMouseClickProgress();
    }

    public void EndMainAction()
    {
        _mainAction.OnMouseClickEnd();
    }

    public void StartSubAction() 
    {
        _subAction.OnMouseClickStart();
    }

    public void ProgressSubAction()
    {
        _subAction.OnMouseClickProgress();
    }

    public void EndSubAction() 
    {
        _subAction.OnMouseClickEnd(); 
    }

    public virtual bool CanReload() { return false; }

    public virtual void OnReload() { }

    public virtual void ReloadAmmo() { }

    public virtual float ReturnReloadFinishTime() { return default; }

    public virtual float ReturnReloadStateExitTime() { return default; }


    // ���⿡ �־ ó������

    protected virtual void ChainMainActionStartEvent() { }
    protected virtual void ChainMainActionProgressEvent() { }
    protected virtual void ChainMainActionEndEvent() { }


    protected virtual void ChainSubActionStartEvent() { }
    protected virtual void ChainSubActionProgressEvent() { }
    protected virtual void ChainSubActionEndEvent() { }
}