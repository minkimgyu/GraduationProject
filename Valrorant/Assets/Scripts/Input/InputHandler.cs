using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class InputHandler : MonoBehaviour
{
    public enum Type
    {
        Sit,
        Stand,

        Jump,
        Walk,
        Stop,

        EventStart,
        EventEnd,

        Equip,

        Reload,
        Drop,
        Interact,

        BuildFormation,
        FreeRole,
        PickUpWeapon,
        SetPriorityTarget,
    }

    private static InputHandler instance = null;

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static InputHandler Instance { get { return instance; } }

    //// input 이벤트가 하나가 아닐 수도 있음
    Dictionary<Type, BaseCommand> _commands = new Dictionary<Type, BaseCommand>();

    public static void AddInputEvent(Type type, BaseCommand command)
    {
        instance._commands.Add(type, command);
    }
    public static void RemoveInputEvent(Type type)
    {
        instance._commands.Remove(type);
    }

    void ExecuteCommand(Type type)
    {
        if (instance._commands.ContainsKey(type) == false) return;
        instance._commands[type].Execute();
    }

    void ExecuteCommand(Type type, Vector3 dir)
    {
        if (instance._commands.ContainsKey(type) == false) return;
        instance._commands[type].Execute(dir);
    }

    void ExecuteCommand(Type type, BaseWeapon.EventType eventType)
    {
        if (instance._commands.ContainsKey(type) == false) return;
        instance._commands[type].Execute(eventType);
    }

    void ExecuteCommand(Type type, BaseWeapon.Type equipType)
    {
        if (instance._commands.ContainsKey(type) == false) return;
        instance._commands[type].Execute(equipType);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl)) instance.ExecuteCommand(Type.Sit);
        else if(Input.GetKeyUp(KeyCode.LeftControl)) instance.ExecuteCommand(Type.Stand);

        Vector3 dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        if(dir != Vector3.zero) instance.ExecuteCommand(Type.Walk, dir);

        if (dir == Vector3.zero) instance.ExecuteCommand(Type.Stop);

        if (Input.GetKeyDown(KeyCode.Space)) instance.ExecuteCommand(Type.Jump);

        if(Input.GetMouseButtonDown(0)) instance.ExecuteCommand(Type.EventStart, BaseWeapon.EventType.Main);
        else if (Input.GetMouseButtonUp(0)) instance.ExecuteCommand(Type.EventEnd);

        if (Input.GetMouseButtonDown(1)) instance.ExecuteCommand(Type.EventStart, BaseWeapon.EventType.Sub);
        else if (Input.GetMouseButtonUp(1)) instance.ExecuteCommand(Type.EventEnd);

        if (Input.GetKeyDown(KeyCode.Alpha1)) instance.ExecuteCommand(Type.Equip, BaseWeapon.Type.Main);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) instance.ExecuteCommand(Type.Equip, BaseWeapon.Type.Sub);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) instance.ExecuteCommand(Type.Equip, BaseWeapon.Type.Melee);

        if (Input.GetKeyDown(KeyCode.R)) instance.ExecuteCommand(Type.Reload);
        if (Input.GetKeyDown(KeyCode.G)) instance.ExecuteCommand(Type.Drop);
        if (Input.GetKeyDown(KeyCode.F)) instance.ExecuteCommand(Type.Interact);


        if (Input.GetKeyDown(KeyCode.B)) 
            instance.ExecuteCommand(Type.BuildFormation);
        if (Input.GetKeyDown(KeyCode.I)) 
            instance.ExecuteCommand(Type.FreeRole);
    }
}
