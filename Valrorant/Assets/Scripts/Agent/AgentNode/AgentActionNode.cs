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
        if (Input.GetMouseButtonDown(0)) _state = NodeState.SUCCESS; // 이렇게 받지 말고 사격 시작 / 끝으로 구분해서 발사 적용시키자
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
        if (Input.GetMouseButtonUp(0)) _state = NodeState.SUCCESS; // 이렇게 받지 말고 사격 시작 / 끝으로 구분해서 발사 적용시키자
        else _state = NodeState.FAILURE;

        return _state;
    }
}

public class PlayMainAction : Node
{
    // 이 부분을 옵져버 패턴으로 구현해서 아예 구독시켜서 사용하는 방식으로 만들어보자
    // --> WeaponController에서 변경시켜주는 방식으로 구현

    Agent loadAgent;

    public PlayMainAction(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        // Shoot 적용
        loadAgent.WeaponController.NowEquipedWeapon.OnMainAction();

        // 여기에 넣지 말고 따로 판정하는 부분에서 출력되게끔 해야함

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
        // Shoot 적용
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
        if (Input.GetMouseButtonDown(1)) _state = NodeState.SUCCESS; // 이렇게 받지 말고 사격 시작 / 끝으로 구분해서 발사 적용시키자
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
        if (Input.GetMouseButtonUp(1)) _state = NodeState.SUCCESS; // 이렇게 받지 말고 사격 시작 / 끝으로 구분해서 발사 적용시키자
        else _state = NodeState.FAILURE;

        return _state;
    }
}

public class PlaySubAction : Node
{
    // 이 부분을 옵져버 패턴으로 구현해서 아예 구독시켜서 사용하는 방식으로 만들어보자
    // --> WeaponController에서 변경시켜주는 방식으로 구현

    Agent loadAgent;

    public PlaySubAction(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        // Shoot 적용
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
        // Shoot 적용
        loadAgent.WeaponController.NowEquipedWeapon.OnStopSubAction();

        _state = NodeState.SUCCESS;
        return _state;
    }
}
