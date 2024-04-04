using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Flocking
{

    //public class ObstacleAvoidBehavior : BaseBehavior
    //{
    //    [SerializeField] Transform _raycastStartPoint;
    //    [SerializeField] LayerMask _layer;

    //    public override Vector3 ReturnVelocity(List<FlockAgent> nearAgents, List<Transform> nearObstacles)
    //    {
    //        if (nearObstacles.Count == 0) return Vector3.zero;

    //        Vector3 dir = nearObstacles[0].position - transform.position;
    //        Debug.DrawRay(_raycastStartPoint.position, dir, Color.red);

    //        RaycastHit hit;
    //        Physics.Raycast(_raycastStartPoint.position, dir, out hit, 5, _layer);
    //        if(hit.collider == null) return Vector3.zero;

    //        float avoidDistance = 2f;

    //        Vector3 normal = hit.normal;
    //        Vector3 newTarget = normal * avoidDistance + transform.forward;
    //        Debug.DrawRay(_raycastStartPoint.position, newTarget, Color.black);

    //        return newTarget.normalized * _weight;
    //    }
    //}
}