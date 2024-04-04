using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MovementComponent : DisplacementSender
{
    private Rigidbody _rigidbody;
    private Vector3 _moveDirection;

    [SerializeField] Transform _direction;

    [SerializeField] float _slowDownRatioByWeaponWeight = 1;

    [SerializeField] float _moveForce;
    [SerializeField] float _altMoveForce;
    [SerializeField] float _sitMoveForce;

    [SerializeField] float _jumpForce;

    bool _lockToSitMoveForce = false;

    public bool LockToCrouchForce { set { _lockToSitMoveForce = value; } }

    [SerializeField]
    float _crouchDuration;

    [SerializeField]
    CapsuleCollider _capsuleCollider;

    [SerializeField]
    Transform _holder;

    float _capsuleStandCenter = 1f;
    float _capsuleCrouchHeight = 1.7f;

    float _capsuleStandHeight = 2f;
    float _capsuleCrouchCenter = 1.15f;


    float _standHeight = 0f;
    float _crouchHeight = 0f;

    Timer _postureTimer;
    public Timer PostureTimer { get { return _postureTimer; } }


    public void OnWeaponChangeRequested(float slowDownRatio)
    {
        _slowDownRatioByWeaponWeight = slowDownRatio;
        Debug.Log(_slowDownRatioByWeaponWeight);
    }

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _postureTimer = new Timer();
    }

    public override void RaiseDisplacementEvent()
    {
        OnDisplacementRequested?.Invoke(_rigidbody.velocity.magnitude * _velocityLengthDecreaseRatio);
    }

    public void ResetDirection()
    {
        _moveDirection = _direction.TransformVector(new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")));
    }

    public void Jump()
    {
        _rigidbody.AddForce(new Vector3(0, _jumpForce, 0), ForceMode.Impulse);
    }

    public void Move(bool pressAlt = false)
    {
        float moveForce;

        if(_lockToSitMoveForce)
        {
            moveForce = _sitMoveForce;
        }
        else
        {
            if (pressAlt) moveForce = _altMoveForce;
            else moveForce = _moveForce;
        }

        moveForce *= _slowDownRatioByWeaponWeight;

        _rigidbody.AddForce(_moveDirection * moveForce, ForceMode.Force);
    }

    public void SwitchPosture()
    {
        if(_postureTimer.IsRunning == true)
        {
            _postureTimer.Reset();
        }

        _postureTimer.Start(_crouchDuration);
    }
    public void UpdateCrouch()
    {
        _postureTimer.Update();
        ChangeColliderSize(_postureTimer.Ratio, _capsuleCrouchHeight, _capsuleCrouchCenter, _crouchHeight);
    }

    public void UpdateStand()
    {
        _postureTimer.Update();
        ChangeColliderSize(_postureTimer.Ratio, _capsuleStandHeight, _capsuleStandCenter, _standHeight);
    }

    void ChangeColliderSize(float ratio, float capsuleHeight, float capsuleCenter, float holderHeight)
    {
        _capsuleCollider.height = Mathf.Lerp(_capsuleCollider.height, capsuleHeight, ratio);
        _capsuleCollider.center = Vector3.Lerp(_capsuleCollider.center, new Vector3(_capsuleCollider.center.x, capsuleCenter, _capsuleCollider.center.z), ratio);
        _holder.localPosition = Vector3.Lerp(_holder.localPosition, new Vector3(_holder.localPosition.x, holderHeight, _holder.localPosition.z), ratio);
    }
}