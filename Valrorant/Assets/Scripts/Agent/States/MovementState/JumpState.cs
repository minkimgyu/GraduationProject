using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using Agent.Controller;
using System;

namespace Agent.States
{
    public class JumpState : MoveState
    {
        Action RevertToPreviousState;
        float _jumpForce;

        public JumpState(Transform direction, float moveForce, float jumpForce, Rigidbody rigidbody,
            Action<ActionController.MovementState> SetState, Action RevertToPreviousState) 
            : base(direction, moveForce, rigidbody, SetState)
        {
            _jumpForce = jumpForce;
            this.RevertToPreviousState = RevertToPreviousState;
        }

        public override void OnStateCollisionEnter(Collision collision)
        {
            if (collision.transform.tag == "Ground")
            {
                RevertToPreviousState?.Invoke(); // ���� ���·� ������
            }
        }

        public override void OnStateEnter()
        {
            AddForceToRigidBody(Vector3.up, _jumpForce, ForceMode.Impulse);
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

        //    //storedPlayer.MovementComponent.RaiseDisplacementEvent(); // �̵� ���� ���� ������ ���̸� �̹�Ʈ�� �Ѱ���
        //}
    }
}