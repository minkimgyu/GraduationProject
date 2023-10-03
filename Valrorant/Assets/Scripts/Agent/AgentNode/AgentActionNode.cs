using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class IsPressingAnyKey : Node
{
    public override NodeState Evaluate()
    {
        if (Input.anyKeyDown) _state = NodeState.SUCCESS; // 이렇게 받지 말고 사격 시작 / 끝으로 구분해서 발사 적용시키자
        else _state = NodeState.FAILURE;

        return _state;
    }
}



public class CanMainActionStart : Node
{
    public override NodeState Evaluate()
    {
        if (Input.GetMouseButtonDown(0)) _state = NodeState.SUCCESS; // 이렇게 받지 말고 사격 시작 / 끝으로 구분해서 발사 적용시키자
        else _state = NodeState.FAILURE;

        return _state;
    }
}

public class MainActionStart : Node
{
    // 이 부분을 옵져버 패턴으로 구현해서 아예 구독시켜서 사용하는 방식으로 만들어보자
    // --> WeaponController에서 변경시켜주는 방식으로 구현

    Agent loadAgent;

    public MainActionStart(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        // Shoot 적용
        loadAgent.WeaponController.NowEquipedWeapon.OnMainActionStart();

        // 여기에 넣지 말고 따로 판정하는 부분에서 출력되게끔 해야함

        _state = NodeState.SUCCESS;
        return _state;
    }
}

public class MainActionProgress : Node
{
    // 이 부분을 옵져버 패턴으로 구현해서 아예 구독시켜서 사용하는 방식으로 만들어보자
    // --> WeaponController에서 변경시켜주는 방식으로 구현

    Agent loadAgent;

    public MainActionProgress(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        // Shoot 적용

        if (Input.GetMouseButton(0)) loadAgent.WeaponController.NowEquipedWeapon.OnMainActionProgress();
        _state = NodeState.SUCCESS;
        return _state;
    }
}

public class CanMainActionEnd : Node
{
    public override NodeState Evaluate()
    {
        if (Input.GetMouseButtonUp(0)) _state = NodeState.SUCCESS; // 이렇게 받지 말고 사격 시작 / 끝으로 구분해서 발사 적용시키자
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
        // Shoot 적용
        loadAgent.WeaponController.NowEquipedWeapon.OnMainActionEnd();

        _state = NodeState.SUCCESS;
        return _state;
    }
}

public class CanSubActionStart : Node
{
    public override NodeState Evaluate()
    {
        if (Input.GetMouseButtonDown(1)) _state = NodeState.SUCCESS; // 이렇게 받지 말고 사격 시작 / 끝으로 구분해서 발사 적용시키자
        else _state = NodeState.FAILURE;

        return _state;
    }
}

public class SubActionStart : Node
{
    // 이 부분을 옵져버 패턴으로 구현해서 아예 구독시켜서 사용하는 방식으로 만들어보자
    // --> WeaponController에서 변경시켜주는 방식으로 구현

    Agent loadAgent;

    public SubActionStart(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        // Shoot 적용
        loadAgent.WeaponController.NowEquipedWeapon.OnSubActionStart();

        _state = NodeState.SUCCESS;
        return _state;
    }
}

public class SubActionProgress : Node
{
    // 이 부분을 옵져버 패턴으로 구현해서 아예 구독시켜서 사용하는 방식으로 만들어보자
    // --> WeaponController에서 변경시켜주는 방식으로 구현

    Agent loadAgent;

    public SubActionProgress(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        // Shoot 적용
        if (Input.GetMouseButton(1)) loadAgent.WeaponController.NowEquipedWeapon.OnSubActionProgress();

        _state = NodeState.SUCCESS;
        return _state;
    }
}

public class CanSubActionEnd : Node
{
    public override NodeState Evaluate()
    {
        if (Input.GetMouseButtonUp(1)) _state = NodeState.SUCCESS; // 이렇게 받지 말고 사격 시작 / 끝으로 구분해서 발사 적용시키자
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
        // Shoot 적용
        loadAgent.WeaponController.NowEquipedWeapon.OnSubActionEnd();

        _state = NodeState.SUCCESS;
        return _state;
    }
}