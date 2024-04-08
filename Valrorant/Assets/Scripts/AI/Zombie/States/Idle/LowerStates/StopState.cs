using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;

namespace AI.FSM
{
    public class StopState : State
    {
        Action Stop;

        public StopState(Action Stop)
        {
            this.Stop = Stop;
        }

        public override void OnStateEnter()
        {
            Debug.Log("StopState");
        }

        public override void OnStateUpdate()
        {
            Stop?.Invoke();
        }
    }
}