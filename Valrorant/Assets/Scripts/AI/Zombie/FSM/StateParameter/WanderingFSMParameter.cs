using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct WanderingFSMParameter
{
    public WanderingFSMParameter(
        Transform myTrasform, 
        int wanderOffset,
        Action<Vector3> View,
        Action Stop,
        Action<Vector3, bool> FollowPath,
        Func<Vector3, Vector3> ReturnNodePos
        )
    {
        this.myTrasform = myTrasform;
        this.wanderOffset = wanderOffset;

        this.View = View;
        this.Stop = Stop;
        this.FollowPath = FollowPath;

        this.ReturnNodePos = ReturnNodePos;
    }

    public Transform myTrasform { get; }
    public int wanderOffset { get; }

    public Action<Vector3> View { get; }
    public Action Stop { get; }
    public Action<Vector3, bool> FollowPath { get; }

    public Func<Vector3, Vector3> ReturnNodePos { get; }
}
