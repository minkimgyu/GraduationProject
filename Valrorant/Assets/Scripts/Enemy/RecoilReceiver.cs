using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilReceiver : MonoBehaviour
{
    [SerializeField] protected Transform _firePoint;

    [SerializeField] protected Vector2 _firePointRotationMultiplier;

    protected Vector2 _viewRotation;
    protected Vector2 FireViewRotation { get { return _viewRotation + _firePointRotationMultiplier; } } // 총 발사 시작 지점 회전 값 프로퍼티

    public Vector3 ReturnRaycastPos() { return _firePoint.position; }
    public Vector3 ReturnRaycastDir() { return _firePoint.forward; }

    public virtual void OnRecoilRequested(Vector2 recoilForce)
    {
        _firePointRotationMultiplier = recoilForce;
    }

    public virtual void ResetCamera() 
    {
        _firePoint.rotation = Quaternion.Euler(FireViewRotation.y, FireViewRotation.x, 0);
    }
}
