using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AI;

namespace BehaviorTree.Nodes
{
    public class FixViewToFormation : Node
    {
        Func<ISightTarget> ReturnPlayer;

        Action<Vector3> View;
        Transform _myTransform;

        public FixViewToFormation(Transform myTransform, Func<ISightTarget> ReturnPlayer, Action<Vector3> View)
        {
            _myTransform = myTransform;
            this.ReturnPlayer = ReturnPlayer;

            this.View = View;
        }

        public override NodeState Evaluate()
        {
            ISightTarget target = ReturnPlayer();
            Vector3 dir = (_myTransform.position - target.ReturnPos()).normalized;

            View?.Invoke(new Vector3(dir.x, 0, dir.z));
            return NodeState.SUCCESS;
        }
    }
}