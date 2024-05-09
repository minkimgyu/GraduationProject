using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;
using Agent.Component;
using Agent.States;

namespace Agent.Controller
{
    public class ActionController : MonoBehaviour
    {
        public enum PostureState
        {
            Sit,
            Stand
        }

        public enum MovementState
        {
            Stop,
            Walk,
            Jump
        }

        StateMachine<PostureState> _postureFSM;
        StateMachine<MovementState> _movementFSM;

        ViewComponent _viewComponent;
        ZoomComponent _zoomComponent;

        CapsuleCollider _capsuleCollider;
        Rigidbody _rigidbody;

        [SerializeField] Transform _direction;
        [SerializeField] float _walkSpeed;
        [SerializeField] float _walkSpeedOnAir;
        [SerializeField] float _jumpSpeed;

        [SerializeField] float _postureSwitchDuration;
        [SerializeField] float _capsuleStandCenter = 1f;
        [SerializeField] float _capsuleCrouchHeight = 1.7f;

        [SerializeField] float _capsuleStandHeight = 2f;
        [SerializeField] float _capsuleCrouchCenter = 1.15f;

        public void Initialize()
        {
            _capsuleCollider = GetComponent<CapsuleCollider>();
            _rigidbody = GetComponent<Rigidbody>();

            _viewComponent = GetComponent<ViewComponent>();
            _zoomComponent = GetComponent<ZoomComponent>();

            _movementFSM = new StateMachine<MovementState>();
            _postureFSM = new StateMachine<PostureState>();

            Dictionary<PostureState, BaseState> postureStates = new Dictionary<PostureState, BaseState>();

            BaseState stand = new StandState(_capsuleCollider, _postureSwitchDuration, _capsuleStandHeight, _capsuleStandCenter, SetState);
            BaseState sit = new SitState(_capsuleCollider, _postureSwitchDuration, _capsuleCrouchHeight, _capsuleCrouchCenter, SetState);

            postureStates.Add(PostureState.Sit, sit);
            postureStates.Add(PostureState.Stand, stand);

            _postureFSM.Initialize(postureStates);
            _postureFSM.SetState(PostureState.Stand);

            Dictionary<MovementState, BaseState> movementStates = new Dictionary<MovementState, BaseState>();

            BaseState stop = new StopState(SetState);
            BaseState walk = new WalkState(_direction, _walkSpeed, _rigidbody, SetState);
            BaseState jump = new JumpState(_direction, _walkSpeedOnAir, _jumpSpeed, _rigidbody, SetState, RevertToPreviousState);

            movementStates.Add(MovementState.Stop, stop);
            movementStates.Add(MovementState.Walk, walk);
            movementStates.Add(MovementState.Jump, jump);

            _movementFSM.Initialize(movementStates);
            _movementFSM.SetState(MovementState.Stop);
        }

        public void OnHandleSit() => _postureFSM.OnHandleSit();
        public void OnHandleStand() => _postureFSM.OnHandleStand();

        public void OnHandleJump() => _movementFSM.OnHandleJump();
        public void OnHandleStop() => _movementFSM.OnHandleStop();
        public void OnHandleMove(Vector3 dir) => _movementFSM.OnHandleMove(dir);

        public void OnUpdate()
        {
            _viewComponent.ResetView();
            _zoomComponent.OnUpdate();

            _movementFSM.OnUpdate();
            _postureFSM.OnUpdate();
        }

        public void OnFixedUpdate()
        {
            _movementFSM.OnFixedUpdate();
        }

        public void OnLateUpdate()
        {
            _viewComponent.ResetCamera();
        }

        private void OnCollisionEnterRequested(Collision collision)
        {
            _movementFSM.OnCollisionEnter(collision);
        }

        void SetState(PostureState state) { _postureFSM.SetState(state); }
        void SetState(MovementState state) { _movementFSM.SetState(state); }
        void RevertToPreviousState() { _movementFSM.RevertToPreviousState(); }
    }
}