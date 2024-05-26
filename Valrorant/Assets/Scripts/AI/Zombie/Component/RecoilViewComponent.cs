using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI.Component;

public class RecoilViewComponent : ViewComponent, IRecoilReceiver
{
    [SerializeField] protected Transform _firePoint;
    [SerializeField] Transform _sightPoint;

    Vector2 _firePointRotationMultiplier;
    Vector2 _viewRotation;

    public Vector2 FireViewRotation { get { return _viewRotation + _firePointRotationMultiplier; } }

    public void ApplyRecoilToCamera()
    {
        _viewRotation = new Vector2(_sightPoint.rotation.eulerAngles.y, _sightPoint.rotation.eulerAngles.x);
        _firePoint.rotation = Quaternion.Euler(FireViewRotation.y, FireViewRotation.x, 0);
    }

    public void OnRecoilRequested(Vector2 recoilForce)
    {
        _firePointRotationMultiplier = recoilForce;
    }

    public Vector3 ReturnRaycastPos() { return _firePoint.position; }

    public Vector3 ReturnRaycastDir() { return _firePoint.forward; }
}
