using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public abstract class Tree : MonoBehaviour
{
    private Node _rootNode = null;

    protected void Start()
    {
        _rootNode = SetUp();
    }

    private void Update()
    {
        if (_rootNode != null)
            _rootNode.Evaluate();
    }

    protected abstract Node SetUp();
}
