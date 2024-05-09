using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Agent.Controller;
using Agent.Component;

public class Player : DirectDamageTarget, IDamageable, ISightTarget
{
    [SerializeField] Transform _sightPoint;
    [SerializeField] float _maxHp;
    float _hp;

    public TargetType MyType { get; set; }

    InteractionController _interactionController;
    ActionController _actionController;
    WeaponController _weaponController;

    [SerializeField] Animator _ownerAnimator;

    protected override void Start()
    {
        Cursor.visible = false;
        MyType = TargetType.Human;

        base.Start();

        _hp = _maxHp;

        _interactionController = GetComponentInChildren<InteractionController>();
        _interactionController.Initialize();
        InputHandler.AddInputEvent(InputHandler.Type.Interact, new BaseCommand(_interactionController.OnHandleInteract));

        ZoomComponent zoomComponent = GetComponent<ZoomComponent>();
        _weaponController = GetComponent<WeaponController>();
        _weaponController.Initialize(
            false,
            zoomComponent.OnZoomCalled, 
            (name, layer, nomalizedTime) => _ownerAnimator.Play(name, layer, nomalizedTime)
        );

        _weaponController.OnHandleEquip(BaseWeapon.Type.Sub);

        InputHandler.AddInputEvent(InputHandler.Type.Equip, new EquipCommand(_weaponController.OnHandleEquip));
        InputHandler.AddInputEvent(InputHandler.Type.EventStart, new EventCommand(_weaponController.OnHandleEventStart));
        InputHandler.AddInputEvent(InputHandler.Type.EventEnd, new Command(_weaponController.OnHandleEventEnd));

        InputHandler.AddInputEvent(InputHandler.Type.Reload, new Command(_weaponController.OnHandleReload));
        InputHandler.AddInputEvent(InputHandler.Type.Drop, new Command(_weaponController.OnHandleDrop));

        _actionController = GetComponent<ActionController>();
        _actionController.Initialize();

        InputHandler.AddInputEvent(InputHandler.Type.Sit, new Command(_actionController.OnHandleSit));
        InputHandler.AddInputEvent(InputHandler.Type.Stand, new Command(_actionController.OnHandleStand));

        InputHandler.AddInputEvent(InputHandler.Type.Jump, new Command(_actionController.OnHandleJump));
        InputHandler.AddInputEvent(InputHandler.Type.Stop, new Command(_actionController.OnHandleStop));
        InputHandler.AddInputEvent(InputHandler.Type.Walk, new MoveCommand(_actionController.OnHandleMove));

        Commander commander  = GetComponent<Commander>();
        commander.Initialize();

        InputHandler.AddInputEvent(InputHandler.Type.FreeRole, new Command(commander.FreeRole));
        InputHandler.AddInputEvent(InputHandler.Type.BuildFormation, new Command(commander.BuildFormation));

        InputHandler.AddInputEvent(InputHandler.Type.PickUpWeapon, new Command(commander.PickUpWeapon));
        InputHandler.AddInputEvent(InputHandler.Type.SetPriorityTarget, new Command(commander.SetPriorityTarget));
    }

    private void Update()
    {
        _actionController.OnUpdate();
        _weaponController.OnUpdate();
    }

    private void FixedUpdate()
    {
        _actionController.OnFixedUpdate();
    }

    private void LateUpdate()
    {
        _actionController.OnLateUpdate();
    }


    public void GetDamage(float damage)
    {
        _hp -= damage;

        if (_hp <= 0)
        {
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus) Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.None;
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

    public bool IsDie()
    {
        return _hp <= 0;
    }

    public void Die()
    {
    }
}
