using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using Agent.Controller;
using System;

namespace Agent.States
{
    public class StandState : PostureState
    {
        public StandState(CapsuleCollider capsuleCollider, float switchDuration, float capsuleStandHeight,
             float capsuleStandCenter, Action<ActionController.PostureState> SetState
             ) : base(capsuleCollider, switchDuration, capsuleStandHeight, capsuleStandCenter, SetState)
        {
        }

        public override void OnHandleSit()
        {
            SetState?.Invoke(ActionController.PostureState.Sit);
        }
    }
}