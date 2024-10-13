using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MoveCameraCommand : BaseCommand
{
    Action<Vector3, Vector3> MoveEvent;

    public MoveCameraCommand(Action<Vector3, Vector3> MoveEvent)
    {
        this.MoveEvent = MoveEvent;
    }

    public override void Execute(Vector3 cameraHolderPosition, Vector3 viewRotation)
    {
        MoveEvent?.Invoke(cameraHolderPosition, viewRotation);
    }
}

public class ChangeFieldOfViewCommand : BaseCommand
{
    Action<float, float> ChangeEvent;

    public ChangeFieldOfViewCommand(Action<float, float> ChangeEvent)
    {
        this.ChangeEvent = ChangeEvent;
    }

    public override void Execute(float fieldOfView, float ratio)
    {
        ChangeEvent?.Invoke(fieldOfView, ratio);
    }
}

public class ActiveCommand : BaseCommand
{
    Action<bool> ActivateEvent;

    public ActiveCommand(Action<bool> ActivateEvent)
    {
        this.ActivateEvent = ActivateEvent;
    }

    public override void Execute(bool active)
    {
        ActivateEvent?.Invoke(active);
    }
}

public class ChangeRatioCommand : BaseCommand
{
    Action<float> ChangeRatio;

    public ChangeRatioCommand(Action<float> ChangeRatio)
    {
        this.ChangeRatio = ChangeRatio;
    }

    public override void Execute(float ratio)
    {
        ChangeRatio?.Invoke(ratio);
    }
}

public class ChangeAmmoCommand : BaseCommand
{
    Action<bool, int, int> ChangeAmmo;

    public ChangeAmmoCommand(Action<bool, int, int> ChangeAmmo)
    {
        this.ChangeAmmo = ChangeAmmo;
    }

    public override void Execute(bool turnOn, int current, int total)
    {
        ChangeAmmo?.Invoke(turnOn, current, total);
    }
}

public class ObserverEventBus : BaseBus<ObserverEventBus.Type>
{
    public enum Type
    {
        ChangeHp,
        ChangeAmmo,

        ActiveCrosshair,
        ChangeCameraFieldOfView,
        MoveCamera,
    }

    public override void Publish(Type state, float ratio)
    {
        if (_commands.ContainsKey(state) == false) return;
        _commands[state].Execute(ratio);
    }

    public override void Publish(Type state, bool turnOn)
    {
        if (_commands.ContainsKey(state) == false) return;
        _commands[state].Execute(turnOn);
    }

    public override void Publish(Type state, Vector3 cameraHolderPosition, Vector3 viewRotation)
    {
        if (_commands.ContainsKey(state) == false) return;
        _commands[state].Execute(cameraHolderPosition, viewRotation);
    }

    public override void Publish(Type state, float fieldOfView, float progress)
    {
        if (_commands.ContainsKey(state) == false) return;
        _commands[state].Execute(fieldOfView, progress);
    }

    public override void Publish(Type state, bool turnOn, int current, int total)
    {
        if (_commands.ContainsKey(state) == false) return;
        _commands[state].Execute(turnOn, current, total);
    }
}
