using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class CanJump : Node
{
    Agent loadAgent;

    public CanJump(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        if (Input.GetKeyDown(KeyCode.Space) && loadAgent.IsLevitating == false) _state = NodeState.SUCCESS;
        else _state = NodeState.FAILURE;

        return _state;
    }
}

public class Jump : Node
{
    Agent loadAgent;

    public Jump(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        loadAgent.Jump();

        _state = NodeState.SUCCESS;
        return _state;
    }
}

public class CanCrouch : Node
{
    public override NodeState Evaluate()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.LeftControl))
        {
            _state = NodeState.SUCCESS;
        }
        else _state = NodeState.FAILURE;

        return _state;
    }
}

public class Crouch : Node
{
    bool nowCrouch = false;

    Agent loadAgent;

    public Crouch(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        nowCrouch = !nowCrouch;
        //loadAgent.Crouch(nowCrouch);

        _state = NodeState.SUCCESS;
        return _state;
    }
}

public class Move : Node
{
    Agent loadAgent;

    public Move(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        loadAgent.ResetDir();

        _state = NodeState.SUCCESS;
        return _state;
    }
}
