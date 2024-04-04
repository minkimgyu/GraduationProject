using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Flocking
{
    //public class StayCenterBehavior : BaseBehavior
    //{
    //    [SerializeField] Vector3 _center = Vector3.zero;
    //    [SerializeField] float radius = 20;

    //    public override Vector3 ReturnVelocity(List<FlockAgent> nearAgents, List<Transform> nearObstacles)
    //    {
    //        Vector3 centerOffset = _center - transform.position;
    //        float t = centerOffset.magnitude / radius;
    //        if (t < 0.9f) return Vector3.zero;

    //        return centerOffset.normalized * _weight;
    //    }
    //}
}