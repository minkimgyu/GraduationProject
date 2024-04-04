using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AI;

public struct IdleStateParameter
{
   public IdleStateParameter(
       float angleOffset,
       float angleChangeAmount,
       int wanderOffset,
       float stateChangeDelay,
       Transform captureTransform,
       Transform myTrasform,

       Action<Zombie.State> SetState,
       Action<Vector3, bool> FollowPath,
       Action<Vector3> View,
       Action Stop,
       Func<bool> IsTargetInSight, 
       Func<Vector3, Vector3> ReturnNodePos
       )
   {
        this.angleOffset = angleOffset;
        this.angleChangeAmount = angleChangeAmount;
        this.wanderOffset = wanderOffset;
        this.stateChangeDelay = stateChangeDelay;
        this.captureTransform = captureTransform;
        this.myTrasform = myTrasform;

        this.SetState = SetState;
        this.FollowPath = FollowPath;
        this.View = View;
        this.Stop = Stop;

        this.IsTargetInSight = IsTargetInSight;
        this.ReturnNodePos = ReturnNodePos;
   }

    public float angleOffset { get; }
    public float angleChangeAmount { get; }
    public int wanderOffset { get; }
    public float stateChangeDelay { get; }
    public Transform captureTransform { get; }
    public Transform myTrasform { get; }

    public Action<Zombie.State> SetState { get; }
    public Action<Vector3, bool> FollowPath { get; }
    public Action<Vector3> View { get; }
    public Action Stop { get; }
    public Func<bool> IsTargetInSight { get; }
    public Func<Vector3, Vector3> ReturnNodePos { get; }
}
