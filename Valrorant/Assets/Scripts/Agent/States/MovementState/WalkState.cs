using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using Agent.Controller;
using System;

namespace Agent.States
{
    public class WalkState : MoveState
    {
        public WalkState(Transform direction, float moveForce, Rigidbody rigidbody, 
            Action<ActionController.MovementState> SetState) : base(direction, moveForce, rigidbody, SetState)
        {
        }

        //public override void OnStateFixedUpdate()
        //{
        //    storedPlayer.MovementComponent.Move();
        //}

        //public override void OnStateLateUpdate()
        //{
        //    storedPlayer.ViewComponent.ResetCamera();
        //}

        //public override void OnStateUpdate()
        //{
        //    storedPlayer.MovementComponent.ResetDirection();
        //    storedPlayer.ViewComponent.ResetView();

        //    //storedPlayer.MovementComponent.RaiseDisplacementEvent(); // 이동 값에 따른 백터의 길이를 이밴트로 넘겨줌
        //}

        public override void OnHandleStop()
        {
            SetState?.Invoke(ActionController.MovementState.Stop);
        }

        public override void OnHandleJump()
        {
            SetState?.Invoke(ActionController.MovementState.Jump);
        }

        //public override void CheckStateChange()
        //{
        //    if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
        //    {
        //        storedPlayer.MovementFSM.SetState(PlayerController.MovementState.Stop);
        //    }

        //    if (((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)) && Input.GetKeyDown(KeyCode.LeftAlt))
        //    {
        //        storedPlayer.MovementFSM.SetState(PlayerController.MovementState.Creep);
        //    }

        //    if (Input.GetKeyDown(KeyCode.Space))
        //    {
        //        storedPlayer.MovementFSM.SetState(PlayerController.MovementState.Jump);
        //    }
        //}
    }
}