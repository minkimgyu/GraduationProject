using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;
using Agent.Controller;

namespace Agent.States
{
    public class PostureState : State
    {
        protected float _capsuleStandHeight;
        protected float _capsuleStandCenter;
        protected float _switchDuration;

        CapsuleCollider _capsuleCollider;
        protected StopwatchTimer _postureTimer;
        protected Action<ActionController.PostureState> SetState;

        public PostureState(CapsuleCollider capsuleCollider, float switchDuration, float capsuleStandHeight,
                float capsuleStandCenter, Action<ActionController.PostureState> SetState)
        {
            _capsuleCollider = capsuleCollider;

            _switchDuration = switchDuration;
            _capsuleStandHeight = capsuleStandHeight;
            _capsuleStandCenter = capsuleStandCenter;

            this.SetState = SetState;
            _postureTimer = new StopwatchTimer();
        }

        public override void OnStateEnter()
        {
            if(_postureTimer.CurrentState == StopwatchTimer.State.Finish) _postureTimer.Reset();
            _postureTimer.Start(_switchDuration);
        }


        public override void OnStateUpdate()
        {
            if (_postureTimer.CurrentState == StopwatchTimer.State.Finish || _postureTimer.CurrentState == StopwatchTimer.State.Ready) return;
            ChangePosture(_postureTimer.Ratio, _capsuleStandHeight, _capsuleStandCenter);
        }

        protected void ChangePosture(float ratio, float capsuleHeight, float capsuleCenter)
        {
            Vector3 center = new Vector3(_capsuleCollider.center.x, capsuleCenter, _capsuleCollider.center.z);
            _capsuleCollider.height = Mathf.Lerp(_capsuleCollider.height, capsuleHeight, ratio);
            _capsuleCollider.center = Vector3.Lerp(_capsuleCollider.center, center, ratio);
        }
    }
}