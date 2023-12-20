using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    public class Sequence : Node
    {
        public Sequence() : base() { }
        public Sequence(List<Node> childNodes) : base(childNodes) { }

        public override NodeState Evaluate()
        {
            bool childNodeIsRunning = false;

            for (int i = 0; i < _childNodes.Count; i++)
            {
                NodeState nowState = _childNodes[i].Evaluate();

                if (nowState == NodeState.FAILURE)
                {
                    return NodeState.FAILURE;
                }
                else if (nowState == NodeState.SUCCESS)
                {
                    continue;
                }
                else if (nowState == NodeState.RUNNING)
                {
                    childNodeIsRunning = true;
                    continue;
                }
                else
                {
                    return NodeState.SUCCESS;
                }
            }

            if (childNodeIsRunning == true)
            {
                return NodeState.RUNNING;
            }
            else
            {
                return NodeState.SUCCESS;
            }
        }
    }

    public class Selector : Node
    {
        public Selector() : base() { }
        public Selector(List<Node> childNodes) : base(childNodes) { }

        public override NodeState Evaluate()
        {
            for (int i = 0; i < _childNodes.Count; i++)
            {
                NodeState nowState = _childNodes[i].Evaluate();

                if (nowState == NodeState.FAILURE)
                {
                    continue;
                }
                else if (nowState == NodeState.SUCCESS)
                {
                    return NodeState.SUCCESS;
                }
                else if (nowState == NodeState.RUNNING)
                {
                    return NodeState.RUNNING;
                }
                else
                {
                    continue;
                }
            }

            return NodeState.FAILURE;
        }
    }

    abstract public class IFNode : Node
    {
        public IFNode(Node childNode) : base(childNode)
        {
        }

        protected abstract bool CheckCondition();

        public override NodeState Evaluate()
        {
            bool result = CheckCondition();

            if (_childNodes.Count == 0 || _childNodes[0] == null) return NodeState.FAILURE;

            if (result == true)
            {
                return _childNodes[0].Evaluate(); // 첫번째 노드를 평가함
            }
            else
            {
                return NodeState.FAILURE;
            }
        }
    }

    abstract public class Node
    {
        Node _parentNode;
        Node ParentNode {set { _parentNode = value; } }

        protected List<Node> _childNodes = new List<Node>();

        public Node()
        {
            _parentNode = null;
        }

        public Node(Node node)
        {
            AddNodeToChildList(node);
        }

        public Node(List<Node> childrenNodes)
        {
            for (int i = 0; i < childrenNodes.Count; i++)
            {
                AddNodeToChildList(childrenNodes[i]);
            }
        }

        void AddNodeToChildList(Node node)
        {
            node.ParentNode = this;
            _childNodes.Add(node);
        }

        public virtual NodeState Evaluate() { return default; }
    }
}