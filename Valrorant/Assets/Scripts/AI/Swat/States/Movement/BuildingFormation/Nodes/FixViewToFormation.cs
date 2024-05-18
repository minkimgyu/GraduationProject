using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AI;

namespace BehaviorTree.Nodes
{
    public class FixViewToFormation : Node
    {
        Func<Vector3> ReturnPlayerPos;

        Action<Vector3> View;
        Transform _myTransform;

        public FixViewToFormation(Transform myTransform, Func<Vector3> ReturnPlayerPos, Action<Vector3> View)
        {
            _myTransform = myTransform;
            this.ReturnPlayerPos = ReturnPlayerPos;

            this.View = View;
        }

        public override NodeState Evaluate()
        {
            Vector3 playerPos = ReturnPlayerPos();
            Vector3 dir = (_myTransform.position - playerPos).normalized;

            View?.Invoke(new Vector3(dir.x, 0, dir.z));
            return NodeState.SUCCESS;
        }
    }
}