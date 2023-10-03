using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class CanReload : Node
{
    Agent loadAgent;

    public CanReload(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        if (Input.GetKeyDown(KeyCode.R)) _state = NodeState.SUCCESS; // �̷��� ���� ���� ��� ���� / ������ �����ؼ� �߻� �����Ű��
        else _state = NodeState.FAILURE;

        return _state;
    }
}

public class Reload : Node
{
    Agent loadAgent;

    public Reload(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        loadAgent.WeaponController.NowEquipedWeapon.OnReload();

        _state = NodeState.SUCCESS;
        return _state;
    }
}
