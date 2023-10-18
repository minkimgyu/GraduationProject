using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObserverPattern;

public class MovementComponent : MonoBehaviour, ISubject<float>
{
    private Rigidbody _rigidbody;
    private Vector3 _moveDirection;

    private float velocityLengthDecreaseRatio = 0.1f;
    public float velocityLength { get { return _rigidbody.velocity.magnitude * velocityLengthDecreaseRatio; } }

    [SerializeField] Transform direction;

    [SerializeField] float _moveForce;
    [SerializeField] float _altMoveForce;
    [SerializeField] float _sitMoveForce;

    [SerializeField] float _jumpForce;

    bool lockToSitMoveForce = false;

    public bool LockToCrouchForce { set { lockToSitMoveForce = value; } }

    public List<IObserver<float>> Observers { get; set; }

    [SerializeField]
    float _crouchDuration;

    [SerializeField]
    CapsuleCollider capsuleCollider;

    Coroutine _crouchCoroutine;

    protected float smoothness = 0.001f;

    WaitForSeconds waitTime;

    [SerializeField]
    Transform holder;


    float capsuleCrouchHeight = 1.7f;
    float capsuleCrouchCenter = 1.15f;


    float capsuleStandHeight = 2f;
    float capsuleStandCenter = 1f;

    float standHeight = 0f;
    float crouchHeight = 0f;
    

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        waitTime = new WaitForSeconds(smoothness);
        Observers = new List<IObserver<float>>();
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

    public void Crouch(bool nowCrouch)
    {
        if (_crouchCoroutine != null) StopCoroutine(_crouchCoroutine);
        _crouchCoroutine = StartCoroutine(CrouchRoutine(nowCrouch));
    }

    IEnumerator CrouchRoutine(bool nowCrouch)
    {
        float progress = 0; //This float will serve as the 3rd parameter of the lerp function.
        float increment = smoothness / _crouchDuration; //The amount of change to apply.
        while (progress < 1)
        {
            if(nowCrouch)
            {
                capsuleCollider.height = Mathf.Lerp(capsuleCollider.height, capsuleCrouchHeight, progress);
                capsuleCollider.center = Vector3.Lerp(capsuleCollider.center, new Vector3(capsuleCollider.center.x, capsuleCrouchCenter, capsuleCollider.center.z), progress);


                holder.localPosition = Vector3.Lerp(holder.localPosition, new Vector3(holder.localPosition.x, crouchHeight, holder.localPosition.z), progress);
            }
            else
            {
                capsuleCollider.height = Mathf.Lerp(capsuleCollider.height, capsuleStandHeight, progress);
                capsuleCollider.center = Vector3.Lerp(capsuleCollider.center, new Vector3(capsuleCollider.center.x, capsuleStandCenter, capsuleCollider.center.z), progress);

                holder.localPosition = Vector3.Lerp(holder.localPosition, new Vector3(holder.localPosition.x, standHeight, holder.localPosition.z), progress);
            }

            progress += increment;
            yield return new WaitForSeconds(smoothness);
        }
    }

    public void AddObserver(IObserver<float> observer)
    {
        Observers.Add(observer);
    }

    public void RemoveObserver(IObserver<float> observer)
    {
        Observers.Remove(observer);
    }

    public void NotifyToObservers(float velocity)
    {
        for (int i = 0; i < Observers.Count; i++)
        {
            Observers[i].Notify(velocity);
        }
    }
}