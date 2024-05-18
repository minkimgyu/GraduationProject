using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct WeaponEventBlackboard 
{
    public WeaponEventBlackboard(Action<bool, float, Vector3, float> OnZoomRequested, Func<float> SendMoveDisplacement,
        Action<Vector2> OnRecoilRequested, Action<string, int, float> OnPlayOwnerAnimation, Func<Vector3> ReturnRaycastPos, 
        Func<Vector3> ReturnRaycastDir, Action<bool, int, int> OnShowRounds)
    {
        this.InvokeZoom = OnZoomRequested;
        this.OnPlayOwnerAnimation = OnPlayOwnerAnimation;
        this.SendMoveDisplacement = SendMoveDisplacement;
        this.OnRecoilRequested = OnRecoilRequested;

        this.ReturnRaycastPos = ReturnRaycastPos;
        this.ReturnRaycastDir = ReturnRaycastDir;

        this.OnShowRounds = OnShowRounds;
    }

    public Action<bool, float, Vector3, float> InvokeZoom { get; }
    public Action<string, int, float> OnPlayOwnerAnimation { get; }
    public Func<float> SendMoveDisplacement { get; set; }
    public Action<Vector2> OnRecoilRequested { get; }

    public Func<Vector3> ReturnRaycastPos { get; }
    public Func<Vector3> ReturnRaycastDir { get; }

    public Action<bool, int, int> OnShowRounds { get; }
}
