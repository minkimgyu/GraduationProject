using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class CanPlayMainAction : Node
{
    Agent loadAgent;

    public CanPlayMainAction(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        if (Input.GetMouseButtonDown(0)) _state = NodeState.SUCCESS; // �̷��� ���� ���� ��� ���� / ������ �����ؼ� �߻� �����Ű��
        else _state = NodeState.FAILURE;

        return _state;
    }
}

public class CanStopMainAction : Node
{
    Agent loadAgent;

    public CanStopMainAction(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        if (Input.GetMouseButtonUp(0)) _state = NodeState.SUCCESS; // �̷��� ���� ���� ��� ���� / ������ �����ؼ� �߻� �����Ű��
        else _state = NodeState.FAILURE;

        return _state;
    }
}

public class PlayMainAction : Node
{
    // �� �κ��� ������ �������� �����ؼ� �ƿ� �������Ѽ� ����ϴ� ������� ������
    // --> WeaponController���� ��������ִ� ������� ����

    Agent loadAgent;

    public PlayMainAction(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        // Shoot ����
        loadAgent.WeaponController.NowEquipedWeapon.OnMainAction();

        // ���⿡ ���� ���� ���� �����ϴ� �κп��� ��µǰԲ� �ؾ���

        _state = NodeState.SUCCESS;
        return _state;
    }
}

public class StopMainAction : Node
{
    Agent loadAgent;

    public StopMainAction(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        // Shoot ����
        loadAgent.WeaponController.NowEquipedWeapon.OnStopMainAction();

        _state = NodeState.SUCCESS;
        return _state;
    }
}

public class CanPlaySubAction : Node
{
    Agent loadAgent;

    public CanPlaySubAction(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        if (Input.GetMouseButtonDown(1)) _state = NodeState.SUCCESS; // �̷��� ���� ���� ��� ���� / ������ �����ؼ� �߻� �����Ű��
        else _state = NodeState.FAILURE;

        return _state;
    }
}

public class CanStopSubAction : Node
{
    Agent loadAgent;

    public CanStopSubAction(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        if (Input.GetMouseButtonUp(1)) _state = NodeState.SUCCESS; // �̷��� ���� ���� ��� ���� / ������ �����ؼ� �߻� �����Ű��
        else _state = NodeState.FAILURE;

        return _state;
    }
}

public class PlaySubAction : Node
{
    // �� �κ��� ������ �������� �����ؼ� �ƿ� �������Ѽ� ����ϴ� ������� ������
    // --> WeaponController���� ��������ִ� ������� ����

    Agent loadAgent;

    public PlaySubAction(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        // Shoot ����
        loadAgent.WeaponController.NowEquipedWeapon.OnDoSubAction();

        _state = NodeState.SUCCESS;
        return _state;
    }
}

public class StopSubAction : Node
{
    Agent loadAgent;

    public StopSubAction(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        // Shoot ����
        loadAgent.WeaponController.NowEquipedWeapon.OnStopSubAction();

        _state = NodeState.SUCCESS;
        return _state;
    }
}
