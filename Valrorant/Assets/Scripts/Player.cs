using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class Player : Tree
{
    protected override Node SetUp()
    {
        List<Node> tmp = new List<Node> { new CanMove(), new Move() };

        Node root = new Selector
        {
            new List<Node>{
                new Sequence(new List<Node> { new CanJump(), new Move() }),
                new Sequence(new List<Node> { new CanCrouch(), new Move() }),
                new Sequence(new List<Node> { new CanMove(), new Move() }),
            }
        }

        return root;
    }
}

public class CanJump : Node
{
    public override NodeState Evaluate()
    {
        if (Input.GetKeyDown(KeyCode.Space)) _state = NodeState.SUCCESS;
        else _state = NodeState.FAILURE;

        return _state;
    }
}

public class Jump : Node
{
    public override NodeState Evaluate()
    {
        Debug.Log("Jump");

        _state = NodeState.RUNNING;
        return _state;
    }
}

public class CanCrouch : Node
{
    public override NodeState Evaluate()
    {
        if (Input.GetKey(KeyCode.LeftControl)) _state = NodeState.SUCCESS;
        else _state = NodeState.FAILURE;

        return _state;
    }
}

public class Crouch : Node
{
    public override NodeState Evaluate()
    {
        Debug.Log("Crouch");

        _state = NodeState.RUNNING;
        return _state;
    }
}

public class CanMove : Node
{
    public override NodeState Evaluate()
    {
        if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) _state = NodeState.SUCCESS;
        else _state = NodeState.FAILURE;

        return _state;
    }
}

public class Move : Node
{
    public override NodeState Evaluate()
    {
        Debug.Log("Move");

        _state = NodeState.RUNNING;
        return _state;
    }
}
