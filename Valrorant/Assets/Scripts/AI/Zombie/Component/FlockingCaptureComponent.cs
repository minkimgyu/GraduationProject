using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingCaptureComponent : MovableTargetCaptureComponent<IFlockingTarget>
{
    IFlockingTarget _flockingTarget;

    public bool HaveLeaderTargetInNearRange(ITarget target)
    {
        for (int i = 0; i < _capturedTargets.Count; i++)
        {
            if (_capturedTargets[i].IsLeader() && _capturedTargets[i].HasSameTarget(target))
            {
                _flockingTarget = _capturedTargets[i];
            }
            return true;

        }

        return false;
    }

    public List<IFlockingTarget> ReturnNearFlockingTargets() { return _capturedTargets; }

    public IFlockingTarget ReturnLeader() { return _flockingTarget; }
}