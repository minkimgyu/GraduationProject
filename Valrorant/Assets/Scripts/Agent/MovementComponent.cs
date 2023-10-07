using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementComponent : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Vector3 _moveDirection;

    [SerializeField] Transform direction;
    [SerializeField] float _moveForce;
    [SerializeField] float _altMoveForce;
    [SerializeField] float _jumpForce;

    private bool _isLevitating = false;
    private bool _isCrouching = false;

    

    [SerializeField]
    float _crouchDuration;

    [SerializeField]
    CapsuleCollider capsuleCollider;

    Coroutine _crouchCoroutine;

    protected float smoothness = 0.001f;

    WaitForSeconds waitTime;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        waitTime = new WaitForSeconds(smoothness);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Ground" && _isLevitating == true)
        {
            _isLevitating = false;
        }
    }
    public bool CanMove()
    {
        return Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;
    }

    public bool CanJump()
    {
        return Input.GetKeyDown(KeyCode.Space) && _isLevitating == false;
    }

    public bool CanCrouch()
    {
        return Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift);
    }

    public void ResetDirection()
    {
        _moveDirection = direction.TransformVector(new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")));
    }

    public void Jump()
    {
        _isLevitating = true;
        _rigidbody.AddForce(new Vector3(0, _jumpForce, 0), ForceMode.Impulse);
    }

    public void Move(bool pressAlt)
    {
        float moveForce;

        if (pressAlt) moveForce = _altMoveForce;
        else moveForce = _moveForce;

        _rigidbody.AddForce(_moveDirection * moveForce, ForceMode.Force);
    }

    public void Crouch()
    {
        _isCrouching = !_isCrouching;
        _crouchCoroutine = StartCoroutine(CrouchRoutine(_isCrouching));
    }

    IEnumerator CrouchRoutine(bool nowCrouch)
    {
        float progress = 0; //This float will serve as the 3rd parameter of the lerp function.
        float increment = smoothness / _crouchDuration; //The amount of change to apply.
        while (progress < 1)
        {
            capsuleCollider.center = Mathf.Lerp()
            capsuleCollider.height

           
            progress += increment;
            yield return new WaitForSeconds(smoothness);
        }
    }
}
