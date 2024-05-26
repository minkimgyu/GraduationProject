using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class AimComponent : RecoilReceiver
//{
//    Quaternion lookRotation;

//    public void AimToPoint(Transform point)
//    {
//        Vector3 tmpDirection = (new Vector3(point.position.x, _firePoint.position.y, point.position.z) - _firePoint.position).normalized;
//        lookRotation = Quaternion.LookRotation(tmpDirection);
//    }

//    public override void ResetCamera()
//    {
//        _firePoint.rotation = lookRotation * Quaternion.Euler(_firePointRotationMultiplier.y, _firePointRotationMultiplier.x, 0);
//    }
//}
