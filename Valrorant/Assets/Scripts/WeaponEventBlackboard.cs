using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct WeaponEventBlackboard 
{
    public WeaponEventBlackboard(Action<bool, float, Vector3, float> OnZoomRequested, Action<float> OnDisplacementRequested,
        Action<Vector2> OnRecoilRequested, Action<string, int, float> OnPlayOwnerAnimation, Func<Vector3> ReturnRaycastPos, Func<Vector3> ReturnRaycastDir)
    {
        this.InvokeZoom = OnZoomRequested;
        this.OnPlayOwnerAnimation = OnPlayOwnerAnimation;
        this.OnDisplacementRequested = OnDisplacementRequested;
        this.OnRecoilRequested = OnRecoilRequested;

        this.ReturnRaycastPos = ReturnRaycastPos;
        this.ReturnRaycastDir = ReturnRaycastDir;
    }

    public Action<bool, float, Vector3, float> InvokeZoom { get; }
    public Action<string, int, float> OnPlayOwnerAnimation { get; }
    public Action<float> OnDisplacementRequested { get; set; }
    public Action<Vector2> OnRecoilRequested { get; }

    public Func<Vector3> ReturnRaycastPos { get; }
    public Func<Vector3> ReturnRaycastDir { get; }
}
