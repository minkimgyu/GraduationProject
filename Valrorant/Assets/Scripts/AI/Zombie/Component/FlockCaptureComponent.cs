using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree.Nodes
{
    //public class FlockCaptureComponent : BaseCaptureComponent
    //{
    //    [SerializeField] float _captureAngle = 90;

    //    [SerializeField] BaseDrawer _circularSectorDrawer;
    //    [SerializeField] Transform _sightPoint;

    //    [SerializeField] List<Transform> _capturedTransforms = new List<Transform>();

    //    [SerializeField] Transform _storedTarget;

    //    public override void Initialize(float radius)
    //    {
    //        _captureRadius = radius;
    //    }

    //    protected override void OnTargetEnter(Collider other)
    //    {
    //        _capturedTransforms.Add(other.transform);
    //    }

    //    protected override void OnTargetExit(Collider other)
    //    {
    //        _capturedTransforms.Remove(other.transform);
    //    }

    //    protected override bool IsAlreadyContaining(Collider other)
    //    {
    //        return _capturedTransforms.Contains(other.transform);
    //    }
    //}
}
