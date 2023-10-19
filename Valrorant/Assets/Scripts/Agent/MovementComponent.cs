using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObserverPattern;
using System;

public class MovementComponent : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Vector3 _moveDirection;

    private float velocityLengthDecreaseRatio = 0.1f;

    [SerializeField] Transform direction;

    [SerializeField] float _moveForce;
    [SerializeField] float _altMoveForce;
    [SerializeField] float _sitMoveForce;

    [SerializeField] float _jumpForce;

    bool lockToSitMoveForce = false;

    public bool LockToCrouchForce { set { lockToSitMoveForce = value; } }

    [SerializeField]
    float _crouchDuration;

    [SerializeField]
    CapsuleCollider capsuleCollider;

    Coroutine _crouchCoroutine;

    protected float smoothness = 0.001f;

    WaitForSeconds waitTime;

    [SerializeField]
    Transform holder;

    float capsuleStandCenter = 1f;
    float capsuleCrouchHeight = 1.7f;

    float capsuleStandHeight = 2f;
    float capsuleCrouchCenter = 1.15f;


    float standHeight = 0f;
    float crouchHeight = 0f;

    public Action<float> OnDisplacementRequested;

    [SerializeField]
    SmoothLerpUtility _smoothLerpUtility;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        waitTime = new WaitForSeconds(smoothness);

        _smoothLerpUtility.OnLerpRequested += Stand;
    }

    public void RaiseDisplacementEvent()
    {
        OnDisplacementRequested?.Invoke(_rigidbody.velocity.magnitude * velocityLengthDecreaseRatio);
    }

    public void ResetDirection()
    {
        _moveDirection = direction.TransformVector(new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")));
    }

    public void Jump()
    {
        _rigidbody.AddForce(new Vector3(0, _jumpForce, 0), ForceMode.Impulse);
    }

    public void Move(bool pressAlt = false)
    {
        float moveForce;

        if(lockToSitMoveForce)
        {
            moveForce = _sitMoveForce;
        }
        else
        {
            if (pressAlt) moveForce = _altMoveForce;
            else moveForce = _moveForce;
        }

        _rigidbody.AddForce(_moveDirection * moveForce, ForceMode.Force);
    }

    public void ChangePosture(bool nowCrouch)
    {
        if(nowCrouch)
        {
            _smoothLerpUtility.OnLerpRequested -= Stand;
            _smoothLerpUtility.OnLerpRequested += Crouch;
        }
        else
        {
            _smoothLerpUtility.OnLerpRequested += Stand;
            _smoothLerpUtility.OnLerpRequested -= Crouch;
        }

        if (_smoothLerpUtility.IsRunning()) _smoothLerpUtility.StopSmoothLerp();
        _smoothLerpUtility.StartSmoothLerp(_crouchDuration);
    }

    void Crouch(float ratio)
    {
        ChangeColliderSize(ratio, capsuleCrouchHeight, capsuleCrouchCenter, crouchHeight);
    }

    void Stand(float ratio)
    {
        ChangeColliderSize(ratio, capsuleStandHeight, capsuleStandCenter, standHeight);
    }

    void ChangeColliderSize(float ratio, float capsuleHeight, float capsuleCenter, float holderHeight)
    {
        capsuleCollider.height = Mathf.Lerp(capsuleCollider.height, capsuleHeight, ratio);
        capsuleCollider.center = Vector3.Lerp(capsuleCollider.center, new Vector3(capsuleCollider.center.x, capsuleCenter, capsuleCollider.center.z), ratio);


        holder.localPosition = Vector3.Lerp(holder.localPosition, new Vector3(holder.localPosition.x, holderHeight, holder.localPosition.z), ratio);
    }
}