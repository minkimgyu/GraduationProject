using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewCaptureComponent : MovableTargetCaptureComponent<ITarget>
{
    [SerializeField] float _captureAngle = 90;

    [SerializeField] BaseDrawer _circularSectorDrawer;
    [SerializeField] Transform _sightPoint;

    [SerializeField] ITarget _storedTarget;

    public override void Initialize(float radius, float angle)
    {
        _captureRadius = radius;
        _captureAngle = angle;
    }

    bool canRaycastTarget(Transform target)
    {
        Vector3 dir = (target.position - _sightPoint.position).normalized;

        RaycastHit hit;
        Physics.Raycast(_sightPoint.position, dir, out hit, _captureRadius, _layerMask);
        Debug.DrawRay(_sightPoint.position, dir * _captureRadius, Color.yellow);

        if (hit.transform == target) return true;
        else return false;
    }

    bool isInAngle(float angle) { return angle <= _captureAngle / 2 && -_captureAngle / 2 <= angle; }

    // ReturnTargetInSight 사용시 IsTargetInSight 우선 사용
    public ITarget ReturnTargetInSight() { return _storedTarget; }

    public bool IsTargetInSight()
    {
        if (_capturedTargets.Count == 0) return false;

        for (int i = 0; i < _capturedTargets.Count; i++)
        {
            if (_capturedTargets[i] == null) continue;

            Transform targetTransform = _capturedTargets[i].ReturnTransform();

            float angle = returnAngleBetween(targetTransform.position);
            bool inInAngle = isInAngle(angle);
            if (inInAngle == false) continue;

            bool canRaycast = canRaycastTarget(targetTransform);
            if (canRaycast == false) continue;

            _storedTarget = _capturedTargets[i];
            return true;
        }

        return false;
    }

    float returnAngleBetween(Vector3 targetPos)
    {
        Vector3 dir = (new Vector3(targetPos.x, transform.position.y, targetPos.z) - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dir);
        return angle;
    }

    protected override void OnModifyData()
    {
        base.OnModifyData();
        _circularSectorDrawer.ResetData(_captureAngle, _captureRadius);
    }
}