using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using Agent.Controller;
using System;

namespace Agent.States
{
    public class StopState : State
    {
        Action<ActionController.MovementState> SetState;

        public StopState(Action<ActionController.MovementState> SetState)
        {
            this.SetState = SetState;
        }

        public override void OnHandleMove(Vector3 input)
        {
            SetState?.Invoke(ActionController.MovementState.Walk);
        }

        public override void OnHandleJump()
        {
            SetState?.Invoke(ActionController.MovementState.Jump);
        }

        //public override void CheckStateChange()
        //{
        //    if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) && Input.GetKeyDown(KeyCode.LeftAlt))
        //    {
        //        storedPlayer.MovementFSM.SetState(PlayerController.MovementState.Creep);
        //    }

        //    if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0))
        //    {
        //        storedPlayer.MovementFSM.SetState(PlayerController.MovementState.Walk);
        //    }

        //    if (Input.GetKeyDown(KeyCode.Space))
        //    {
        //        storedPlayer.MovementFSM.SetState(PlayerController.MovementState.Jump);
        //    }
        //}

        //public override void OnStateLateUpdate()
        //{
        //    storedPlayer.ViewComponent.ResetCamera();
        //}

        //public override void OnStateUpdate()
        //{
        //    storedPlayer.ViewComponent.ResetView();
        //    //storedPlayer.MovementComponent.RaiseDisplacementEvent(); // 이동 값에 따른 백터의 길이를 이밴트로 넘겨줌
        //}
    }
}