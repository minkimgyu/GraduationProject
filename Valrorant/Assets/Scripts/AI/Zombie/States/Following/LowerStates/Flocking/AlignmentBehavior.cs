using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AI.Flocking
{
    public class AlignmentBehavior : SurroundingBehavior
    {
        public AlignmentBehavior(Transform myTransform, float weight, Func<List<IFlockingTarget>> ReturnNearAgents) 
            : base(myTransform, weight, ReturnNearAgents)
        {
        }

        public override Vector3 ReturnVelocity()
        {
            List<IFlockingTarget> nearAgents = ReturnNearAgents();

            if (nearAgents.Count == 0) return _myTransform.forward * _weight;

            Vector3 fowardDir = new Vector3();
            for (int i = 0; i < nearAgents.Count; i++)
            {
                Transform nearAgentTransform = nearAgents[i].ReturnTransform();
                fowardDir += nearAgentTransform.forward;
            }

            return fowardDir.normalized * _weight;
        }
    }
}
