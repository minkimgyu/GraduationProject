using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class BaseCommand
{
    public BaseCommand(Action DoAction) { }
    public BaseCommand(Action<BaseWeapon.Type> DoAction) { }
    public BaseCommand(Action<BaseWeapon.EventType> DoAction) { }
    public BaseCommand(Action<Vector3> DoAction) { }

    public virtual void Execute() { }
    public virtual void Execute(BaseWeapon.Type type) { }
    public virtual void Execute(BaseWeapon.EventType type) { }
    public virtual void Execute(Vector3 dir) { }
}

public class Command : BaseCommand
{
    Action DoAction;
    public Command(Action DoAction) : base(DoAction)
    {
        this.DoAction = DoAction;
    }

    public override void Execute() 
    {
        DoAction?.Invoke();
    }
}

public class MoveCommand : BaseCommand
{
    Action<Vector3> DoAction;
    public MoveCommand(Action<Vector3> DoAction) : base(DoAction)
    {
        this.DoAction = DoAction;
    }

    public override void Execute(Vector3 dir)
    {
        DoAction?.Invoke(dir);
    }
}


public class EquipCommand : BaseCommand
{
    Action<BaseWeapon.Type> DoAction;
    public EquipCommand(Action<BaseWeapon.Type> DoAction) : base(DoAction)
    {
        this.DoAction = DoAction;
    }

    public override void Execute(BaseWeapon.Type type)
    {
        DoAction?.Invoke(type);
    }
}

public class EventCommand : BaseCommand
{
    Action<BaseWeapon.EventType> DoAction;
    public EventCommand(Action<BaseWeapon.EventType> DoAction) : base(DoAction)
    {
        this.DoAction = DoAction;
    }

    public override void Execute(BaseWeapon.EventType type)
    {
        DoAction?.Invoke(type);
    }
}