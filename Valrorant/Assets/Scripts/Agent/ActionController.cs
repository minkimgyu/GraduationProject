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
        public enum InputState
        {
            Enable,
            Disable
        }

        InputState _inputState;

        public void TurnOnOffInput()
        {
            switch (_inputState)
            {
                case InputState.Enable:
                    _inputState = InputState.Disable;
                    break;
                case InputState.Disable:
                    _inputState = InputState.Enable;
                    break;
            }
        }

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
        float _walkSpeed;
        float _walkSpeedOnAir;
        float _jumpSpeed;

        float _postureSwitchDuration;
        float _capsuleStandCenter = 1f;
        float _capsuleCrouchHeight = 1.7f;

        float _capsuleStandHeight = 2f;
        float _capsuleCrouchCenter = 1.15f;

        public void Initialize(float walkSpeed, float walkSpeedOnAir, float jumpSpeed, float postureSwitchDuration, float capsuleStandCenter,
            float capsuleStandHeight, float capsuleCrouchCenter, float capsuleCrouchHeight, float viewYRange, Vector2 viewSensitivity)
        {
            _walkSpeed = walkSpeed;
            _walkSpeedOnAir = walkSpeedOnAir;
            _jumpSpeed = jumpSpeed;

            _postureSwitchDuration = postureSwitchDuration;
            _capsuleStandCenter = capsuleStandCenter;
            _capsuleCrouchHeight = capsuleCrouchHeight;

            _capsuleCrouchCenter = capsuleCrouchCenter;
            _capsuleStandHeight = capsuleStandHeight;

            _inputState = InputState.Enable;

            _capsuleCollider = GetComponent<CapsuleCollider>();
            _rigidbody = GetComponent<Rigidbody>();

            _viewComponent = GetComponent<ViewComponent>();
            _viewComponent.Initialize(viewYRange, viewSensitivity);

            _zoomComponent = GetComponent<ZoomComponent>();
            _zoomComponent.Initialize();

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

        public void OnHandleSit()
        {
            if (_inputState == InputState.Disable) return;

            _postureFSM.OnHandleSit();
        }

        public void OnHandleStand()
        {
            if (_inputState == InputState.Disable) return;

            _postureFSM.OnHandleStand();
        }

        public void OnHandleJump()
        {
            if (_inputState == InputState.Disable) return;

            _movementFSM.OnHandleJump();
        }

        public void OnHandleStop()
        {
            if (_inputState == InputState.Disable) return;

            _movementFSM.OnHandleStop();
        }

        public void OnHandleMove(Vector3 dir)
        {
            if (_inputState == InputState.Disable) return;

            _movementFSM.OnHandleMove(dir);
        }

        public void OnUpdate()
        {
            _zoomComponent.OnUpdate();

            _movementFSM.OnUpdate();
            _postureFSM.OnUpdate();

            if (_inputState == InputState.Disable) return;
            _viewComponent.ResetView();
        }

        public void OnFixedUpdate()
        {
            _movementFSM.OnFixedUpdate();
        }

        public void OnLateUpdate()
        {
            _viewComponent.ResetCamera();
        }

        public void OnCollisionEnterRequested(Collision collision)
        {
            _movementFSM.OnCollisionEnter(collision);
        }

        void SetState(PostureState state) { _postureFSM.SetState(state); }
        void SetState(MovementState state) { _movementFSM.SetState(state); }
        void RevertToPreviousState() { _movementFSM.RevertToPreviousState(); }
    }
}