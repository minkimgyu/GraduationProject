using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AI.Flocking
{
    abstract public class BaseBehavior
    {
        protected float _weight = 1;
        protected Transform _myTransform;

        public BaseBehavior(Transform myTransform, float weight)
        {
            _myTransform = myTransform; 
            _weight = weight;
        }

        public abstract Vector3 ReturnVelocity();
    }

    abstract public class SurroundingBehavior : BaseBehavior
    {
        protected Func<List<IFlockingTarget>> ReturnNearAgents;

        public SurroundingBehavior(Transform myTransform, float weight, Func<List<IFlockingTarget>> ReturnNearAgents)
            : base(myTransform, weight)
        {
            this.ReturnNearAgents = ReturnNearAgents;
        }
    }
}