using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class CanEquipMainWeapon : Node
{
    Agent loadAgent;

    public CanEquipMainWeapon(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) _state = NodeState.SUCCESS; // �̷��� ���� ���� ��� ���� / ������ �����ؼ� �߻� �����Ű��
        else _state = NodeState.FAILURE;

        return _state;
    }
}

public class EquipMainWeapon : Node
{
    Agent loadAgent;

    public EquipMainWeapon(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        loadAgent.WeaponController.ChangeWeapon(0);

        _state = NodeState.SUCCESS;
        return _state;
    }
}

public class CanEquipSubWeapon : Node
{
    Agent loadAgent;

    public CanEquipSubWeapon(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2)) _state = NodeState.SUCCESS; // �̷��� ���� ���� ��� ���� / ������ �����ؼ� �߻� �����Ű��
        else _state = NodeState.FAILURE;

        return _state;
    }
}

public class EquipSubWeapon : Node
{
    Agent loadAgent;

    public EquipSubWeapon(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        loadAgent.WeaponController.ChangeWeapon(1);

        _state = NodeState.SUCCESS;
        return _state;
    }
}

public class CanEquipKnife : Node
{
    Agent loadAgent;

    public CanEquipKnife(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3)) _state = NodeState.SUCCESS; // �̷��� ���� ���� ��� ���� / ������ �����ؼ� �߻� �����Ű��
        else _state = NodeState.FAILURE;

        return _state;
    }
}

public class EquipKnife : Node
{
    Agent loadAgent;

    public EquipKnife(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        loadAgent.WeaponController.ChangeWeapon(2);

        _state = NodeState.SUCCESS;
        return _state;
    }
}

