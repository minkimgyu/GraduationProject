using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputHandler : MonoBehaviour
{
    public enum Type
    {
        TurnOnOffPlayerRoutine,

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

        Shop,

        BuildFormation,
        FreeRole,
        PickUpWeapon,
        SetPriorityTarget,
    }

    static InputHandler _instance = null;

    //// input 이벤트가 하나가 아닐 수도 있음
    Dictionary<Type, BaseCommand> _commands = new Dictionary<Type, BaseCommand>();

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;

            if (transform.parent != null && transform.root != null)
            {
                DontDestroyOnLoad(transform.root.gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else Destroy(gameObject);
    }

    public static void AddInputEvent(Type type, BaseCommand command)
    {
        _instance._commands.Add(type, command);
    }
    public static void RemoveInputEvent(Type type)
    {
        _instance._commands.Remove(type);
    }

    void ExecuteCommand(Type type)
    {
        if (_instance._commands.ContainsKey(type) == false) return;
        _instance._commands[type].Execute();
    }

    void ExecuteCommand(Type type, Vector3 dir)
    {
        if (_instance._commands.ContainsKey(type) == false) return;
        _instance._commands[type].Execute(dir);
    }

    void ExecuteCommand(Type type, BaseWeapon.EventType eventType)
    {
        if (_instance._commands.ContainsKey(type) == false) return;
        _instance._commands[type].Execute(eventType);
    }

    void ExecuteCommand(Type type, BaseWeapon.Type equipType)
    {
        if (_instance._commands.ContainsKey(type) == false) return;
        _instance._commands[type].Execute(equipType);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl)) _instance.ExecuteCommand(Type.Sit);
        else if(Input.GetKeyUp(KeyCode.LeftControl)) _instance.ExecuteCommand(Type.Stand);

        Vector3 dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        if(dir != Vector3.zero) _instance.ExecuteCommand(Type.Walk, dir);

        if (dir == Vector3.zero) _instance.ExecuteCommand(Type.Stop);

        if (Input.GetKeyDown(KeyCode.Space)) _instance.ExecuteCommand(Type.Jump);

        if(Input.GetMouseButtonDown(0)) _instance.ExecuteCommand(Type.EventStart, BaseWeapon.EventType.Main);
        else if (Input.GetMouseButtonUp(0)) _instance.ExecuteCommand(Type.EventEnd);

        if (Input.GetMouseButtonDown(1)) _instance.ExecuteCommand(Type.EventStart, BaseWeapon.EventType.Sub);
        else if (Input.GetMouseButtonUp(1)) _instance.ExecuteCommand(Type.EventEnd);

        if (Input.GetKeyDown(KeyCode.Alpha1)) _instance.ExecuteCommand(Type.Equip, BaseWeapon.Type.Main);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) _instance.ExecuteCommand(Type.Equip, BaseWeapon.Type.Sub);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) _instance.ExecuteCommand(Type.Equip, BaseWeapon.Type.Melee);

        if (Input.GetKeyDown(KeyCode.R)) _instance.ExecuteCommand(Type.Reload);
        if (Input.GetKeyDown(KeyCode.G)) _instance.ExecuteCommand(Type.Drop);
        if (Input.GetKeyDown(KeyCode.F))
        {
            _instance.ExecuteCommand(Type.Interact);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            _instance.ExecuteCommand(Type.Shop);
            _instance.ExecuteCommand(Type.TurnOnOffPlayerRoutine);
        }


        if (Input.GetKeyDown(KeyCode.P))
            _instance.ExecuteCommand(Type.BuildFormation);
        if (Input.GetKeyDown(KeyCode.L))
            _instance.ExecuteCommand(Type.FreeRole);
    }
}
