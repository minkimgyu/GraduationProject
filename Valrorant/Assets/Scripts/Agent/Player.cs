﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Agent.Controller;
using Agent.Component;
using FSM;
using AI.ZombieFSM;
using System;

public enum LifeState
{
    Alive,
    Die
}

public class Player : DirectDamageTarget, IDamageable, ISightTarget
{
    [SerializeField] Transform _sightPoint;

    public TargetType MyType { get; set; }

    InteractionController _interactionController;
    ActionController _actionController;
    WeaponController _weaponController;

    [SerializeField] Animator _ownerAnimator;

    public enum EventState
    {
        Enable,
        Disable
    }

    EventState _state;

    StateMachine<LifeState> _lifeFsm = new StateMachine<LifeState>();

    public void Initialize(PlayerData data)
    {
        _state = EventState.Enable;

        //Cursor.visible = false;
        MyType = TargetType.Human;

        HpViewer hpViwer = FindObjectOfType<HpViewer>();
        RoundViwer roundViwer = FindObjectOfType<RoundViwer>();

        //(state) => {_lifeFsm.SetState(state); }, hpViwer.OnHpChange)

        _lifeFsm.Initialize(
              new Dictionary<LifeState, BaseState>
              {
                    {LifeState.Alive, new AliveState(data.maxHp, null, hpViwer.OnHpChange)},
                    //{LifeState.Die, new DieState(gameObject, _destoryDelay, ResetAnimatorValue) },
              }
           );
        _lifeFsm.SetState(LifeState.Alive);

        _interactionController = GetComponentInChildren<InteractionController>();
        _interactionController.Initialize();
        InputHandler.AddInputEvent(InputHandler.Type.Interact, new Command(_interactionController.OnHandleInteract));


        Action<BaseWeapon.Name, BaseWeapon.Type> AddWeaponPreview;
        Action<BaseWeapon.Type> RemoveWeaponPreview;

        Shop shop = FindObjectOfType<Shop>();
        shop.AddProfileViewer(data.personName, out AddWeaponPreview, out RemoveWeaponPreview);

        WeaponViewer weaponViewer = FindObjectOfType<WeaponViewer>();

        ZoomComponent zoomComponent = GetComponent<ZoomComponent>();

        _weaponController = GetComponent<WeaponController>();
        _weaponController.Initialize(
            data.weaponThrowPower,
            false,
            zoomComponent.OnZoomCalled,
            (name, layer, nomalizedTime) => _ownerAnimator.Play(name, layer, nomalizedTime),
            roundViwer.OnRoundCountChange,
            null,
            (BaseWeapon.Name name, BaseWeapon.Type type) => { weaponViewer.AddPreview(name, type); AddWeaponPreview?.Invoke(name, type); },
            (BaseWeapon.Type type) => { weaponViewer.RemovePreview(type); RemoveWeaponPreview?.Invoke(type); }
        );

        _weaponController.OnHandleEquip(BaseWeapon.Type.Sub);

        InputHandler.AddInputEvent(InputHandler.Type.Equip, new EquipCommand(_weaponController.OnHandleEquip));
        InputHandler.AddInputEvent(InputHandler.Type.EventStart, new EventCommand(_weaponController.OnHandleEventStart));
        InputHandler.AddInputEvent(InputHandler.Type.EventEnd, new Command(_weaponController.OnHandleEventEnd));

        InputHandler.AddInputEvent(InputHandler.Type.Reload, new Command(_weaponController.OnHandleReload));
        InputHandler.AddInputEvent(InputHandler.Type.Drop, new Command(_weaponController.OnHandleDrop));

        _actionController = GetComponent<ActionController>();
        _actionController.Initialize(data.walkSpeed, data.walkSpeedOnAir, data.jumpSpeed, 
            data.postureSwitchDuration, data.capsuleStandCenter, data.capsuleStandHeight, 
            data.capsuleCrouchCenter, data.capsuleCrouchHeight, data.viewYRange, data.viewSensitivity.V2);

        InputHandler.AddInputEvent(InputHandler.Type.Sit, new Command(_actionController.OnHandleSit));
        InputHandler.AddInputEvent(InputHandler.Type.Stand, new Command(_actionController.OnHandleStand));

        InputHandler.AddInputEvent(InputHandler.Type.Jump, new Command(_actionController.OnHandleJump));
        InputHandler.AddInputEvent(InputHandler.Type.Stop, new Command(_actionController.OnHandleStop));
        InputHandler.AddInputEvent(InputHandler.Type.Walk, new MoveCommand(_actionController.OnHandleMove));

        Commander commander = GetComponent<Commander>();
        commander.Initialize(ReturnPos, data.startRange);

        InputHandler.AddInputEvent(InputHandler.Type.FreeRole, new Command(commander.FreeRole));
        InputHandler.AddInputEvent(InputHandler.Type.BuildFormation, new Command(commander.BuildFormation));

        InputHandler.AddInputEvent(InputHandler.Type.PickUpWeapon, new Command(commander.PickUpWeapon));
        InputHandler.AddInputEvent(InputHandler.Type.SetPriorityTarget, new Command(commander.SetPriorityTarget));

        InputHandler.AddInputEvent(InputHandler.Type.TurnOnOffPlayerRoutine, new Command(TurnOnOffRoutine));


        if (shop == null) return;

        // 상점에 Event를 등록시켜준다.
        shop.AddEvent(Shop.EventType.BuyWeapon, new WeaponCommand(_weaponController.OnWeaponReceived));

        shop.AddEvent(Shop.EventType.BuyWeaponToHelper, new WeaponToHelperCommand(commander.BuyWeaponToListener));
        shop.AddEvent(Shop.EventType.ReviveHelper, new ReviveHelperCommand(commander.ReviveListener));


        // 이 둘은 조력자와 플레이어 모두 적용시켜주기
        shop.AddEvent(Shop.EventType.BuyHealPack, new HealCommand((hp) => { _lifeFsm.OnHeal(hp); commander.HealListeners(hp); }));
        shop.AddEvent(Shop.EventType.BuyAmmo, new Command(() => { _weaponController.RefillAmmo(); commander.RefillAmmoToListeners(); }));
    }

    public bool _isActive = false;

    void TurnOnOffRoutine()
    {
        if (_isActive == false) return;

        switch (_state)
        {
            case EventState.Enable:
                _state = EventState.Disable;
                _actionController.TurnOnOffInput();
                _weaponController.TurnOnOffInput();

                break;
            case EventState.Disable:
                _state = EventState.Enable;
                _actionController.TurnOnOffInput();
                _weaponController.TurnOnOffInput();

                break;
        }
    }

    private void Update()
    {
        if (_state == EventState.Disable) return;

        _actionController.OnUpdate();
        _weaponController.OnUpdate();
    }

    private void FixedUpdate()
    {
        if (_state == EventState.Disable) return;

        _actionController.OnFixedUpdate();
    }

    private void LateUpdate()
    {
        _actionController.OnLateUpdate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        _actionController.OnCollisionEnterRequested(collision);
    }

    public void GetDamage(float damage)
    {
        _lifeFsm.OnDamaged(damage);
    }

    public Vector3 GetFowardVector()
    {
        return transform.forward;
    }

    public Vector3 ReturnPos()
    {
        return transform.position;
    }
    public Transform ReturnTransform()
    {
        return transform;
    }

    public Transform ReturnSightPoint()
    {
        return _sightPoint;
    }

    public bool IsUntrackable()
    {
        return false;
    }
}
