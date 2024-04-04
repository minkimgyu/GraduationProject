using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Flocking
{
    public class FollowBehavior : BaseBehavior
    {
        IFlockingTarget _target;

        public FollowBehavior(Transform myTransform, float weight) : base(myTransform, weight)
        {
        }

        public void ResetData(IFlockingTarget target)
        {
            _target = target;
        }

        public override Vector3 ReturnVelocity()
        {
            Transform targetTransform = _target.ReturnTransform();
            return targetTransform.forward.normalized * _weight;
        }
    }
}