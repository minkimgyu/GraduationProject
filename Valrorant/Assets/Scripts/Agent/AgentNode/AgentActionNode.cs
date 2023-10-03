using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class IsPressingAnyKey : Node
{
    public override NodeState Evaluate()
    {
        if (Input.anyKeyDown) _state = NodeState.SUCCESS; // �̷��� ���� ���� ��� ���� / ������ �����ؼ� �߻� �����Ű��
        else _state = NodeState.FAILURE;

        return _state;
    }
}



public class CanMainActionStart : Node
{
    public override NodeState Evaluate()
    {
        if (Input.GetMouseButtonDown(0)) _state = NodeState.SUCCESS; // �̷��� ���� ���� ��� ���� / ������ �����ؼ� �߻� �����Ű��
        else _state = NodeState.FAILURE;

        return _state;
    }
}

public class MainActionStart : Node
{
    // �� �κ��� ������ �������� �����ؼ� �ƿ� �������Ѽ� ����ϴ� ������� ������
    // --> WeaponController���� ��������ִ� ������� ����

    Agent loadAgent;

    public MainActionStart(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        // Shoot ����
        loadAgent.WeaponController.NowEquipedWeapon.OnMainActionStart();

        // ���⿡ ���� ���� ���� �����ϴ� �κп��� ��µǰԲ� �ؾ���

        _state = NodeState.SUCCESS;
        return _state;
    }
}

public class MainActionProgress : Node
{
    // �� �κ��� ������ �������� �����ؼ� �ƿ� �������Ѽ� ����ϴ� ������� ������
    // --> WeaponController���� ��������ִ� ������� ����

    Agent loadAgent;

    public MainActionProgress(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        // Shoot ����

        if (Input.GetMouseButton(0)) loadAgent.WeaponController.NowEquipedWeapon.OnMainActionProgress();
        _state = NodeState.SUCCESS;
        return _state;
    }
}

public class CanMainActionEnd : Node
{
    public override NodeState Evaluate()
    {
        if (Input.GetMouseButtonUp(0)) _state = NodeState.SUCCESS; // �̷��� ���� ���� ��� ���� / ������ �����ؼ� �߻� �����Ű��
        else _state = NodeState.FAILURE;

        return _state;
    }
}

public class MainActionEnd : Node
{
    Agent loadAgent;

    public MainActionEnd(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        // Shoot ����
        loadAgent.WeaponController.NowEquipedWeapon.OnMainActionEnd();

        _state = NodeState.SUCCESS;
        return _state;
    }
}

public class CanSubActionStart : Node
{
    public override NodeState Evaluate()
    {
        if (Input.GetMouseButtonDown(1)) _state = NodeState.SUCCESS; // �̷��� ���� ���� ��� ���� / ������ �����ؼ� �߻� �����Ű��
        else _state = NodeState.FAILURE;

        return _state;
    }
}

public class SubActionStart : Node
{
    // �� �κ��� ������ �������� �����ؼ� �ƿ� �������Ѽ� ����ϴ� ������� ������
    // --> WeaponController���� ��������ִ� ������� ����

    Agent loadAgent;

    public SubActionStart(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        // Shoot ����
        loadAgent.WeaponController.NowEquipedWeapon.OnSubActionStart();

        _state = NodeState.SUCCESS;
        return _state;
    }
}

public class SubActionProgress : Node
{
    // �� �κ��� ������ �������� �����ؼ� �ƿ� �������Ѽ� ����ϴ� ������� ������
    // --> WeaponController���� ��������ִ� ������� ����

    Agent loadAgent;

    public SubActionProgress(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        // Shoot ����
        if (Input.GetMouseButton(1)) loadAgent.WeaponController.NowEquipedWeapon.OnSubActionProgress();

        _state = NodeState.SUCCESS;
        return _state;
    }
}

public class CanSubActionEnd : Node
{
    public override NodeState Evaluate()
    {
        if (Input.GetMouseButtonUp(1)) _state = NodeState.SUCCESS; // �̷��� ���� ���� ��� ���� / ������ �����ؼ� �߻� �����Ű��
        else _state = NodeState.FAILURE;

        return _state;
    }
}

public class SubActionEnd : Node
{
    Agent loadAgent;

    public SubActionEnd(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        // Shoot ����
        loadAgent.WeaponController.NowEquipedWeapon.OnSubActionEnd();

        _state = NodeState.SUCCESS;
        return _state;
    }
}