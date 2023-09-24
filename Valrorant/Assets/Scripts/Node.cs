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
                    _state = NodeState.FAILURE;
                    return _state;
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
                    _state = NodeState.SUCCESS;
                    return _state;
                }
            }

            if (childNodeIsRunning == true)
            {
                _state = NodeState.RUNNING;
            }
            else
            {
                _state = NodeState.SUCCESS;
            }

            return _state;
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
                    _state = NodeState.SUCCESS;
                    return _state;
                }
                else if (nowState == NodeState.RUNNING)
                {
                    _state = NodeState.RUNNING;
                    return _state;
                }
                else
                {
                    continue;
                }
            }

            _state = NodeState.FAILURE;
            return _state;
        }
    }

    public class Node
    {
        protected NodeState _state;

        Node _parentNode;
        Node ParentNode { get { return _parentNode; } set { _parentNode = value; } }

        protected List<Node> _childNodes = new List<Node>();

        public Node()
        {
            _parentNode = null;
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

        public virtual NodeState Evaluate()
        {
            return NodeState.FAILURE;
        }
    }
}