using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public abstract class Tree : MonoBehaviour
{
    private Node _rootNode = null;

    protected virtual void Start()
    {
        _rootNode = SetUp();
    }

    protected virtual void Update()
    {
        if (_rootNode != null)
            _rootNode.Evaluate();
    }

    protected abstract Node SetUp();
}
