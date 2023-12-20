using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class Tree
{
    private Node _rootNode = null;

    public void SetUp(Node rootNode)
    {
        _rootNode = rootNode;
    }

    public void OnUpdate()
    {
        if (_rootNode != null)
            _rootNode.Evaluate();
    }
}
