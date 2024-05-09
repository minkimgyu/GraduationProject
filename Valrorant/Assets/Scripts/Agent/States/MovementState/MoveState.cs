using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;
using Agent.Controller;

namespace Agent.States
{
    public class MoveState : State
    {
        Transform _direction;
        Vector3 _input;
        float _moveForce;
        protected Rigidbody _rigidbody;
        protected Action<ActionController.MovementState> SetState;

        public MoveState(Transform direction, float moveForce, Rigidbody rigidbody, Action<ActionController.MovementState> SetState)
        {
            _direction = direction;
            _moveForce = moveForce;
            _rigidbody = rigidbody;
            this.SetState = SetState;
        }

        public override void OnHandleStop()
        {
            SetState?.Invoke(ActionController.MovementState.Stop);
        }

        public override void OnHandleMove(Vector3 input)
        {
            _input = input;
        }

        protected void AddForceToRigidBody(Vector3 dir, float force)
        {
            _rigidbody.AddForce(dir * force, ForceMode.Force);
        }

        protected void AddForceToRigidBody(Vector3 dir, float force, ForceMode mode)
        {
            _rigidbody.AddForce(dir * force, mode);
        }

        public override void OnStateFixedUpdate()
        {
            Vector3 moveDir = _direction.TransformVector(_input);
            AddForceToRigidBody(moveDir, _moveForce);
        }
    }
}
