using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using Agent.Controller;
using System;

namespace Agent.States
{
    public class SitState : PostureState
    {
        public SitState(CapsuleCollider capsuleCollider, float switchDuration, float capsuleStandHeight,
            float capsuleStandCenter, Action<ActionController.PostureState> SetState
            ) : base(capsuleCollider, switchDuration, capsuleStandHeight, capsuleStandCenter, SetState)
        {
        }

        public override void OnHandleStand()
        {
            SetState?.Invoke(ActionController.PostureState.Stand);
        }
    }
}