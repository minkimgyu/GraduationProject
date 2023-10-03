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
        if (Input.GetKeyDown(KeyCode.R)) _state = NodeState.SUCCESS; // 이렇게 받지 말고 사격 시작 / 끝으로 구분해서 발사 적용시키자
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
