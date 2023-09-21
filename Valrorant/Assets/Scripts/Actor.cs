using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Actor : MonoBehaviour
{
    [System.Serializable]
    struct ActorSetting
    {
        [Header("View")]
        public float viewXSensitivity;
        public float viewYSensitivity;

        public float viewClampYMax;
        public float viewClampYMin;

        public bool ViewXInverted;
        public bool ViewYInverted;
    }

    private DefaultInput defaultInput;

    Animator _animator;

    [Header("Move")]
    Rigidbody _rigid;

    [SerializeField]
    Vector3 _moveDir;

    private Vector3 smoothInputVelocity;
    private Vector3 currentInputVector;

    private float smoothInputSpeed = 0.2f;

    float _moveForce = 30f;

    [Header("Camera")]

    [SerializeField]
    Transform cameraHolder;

    [SerializeField]
    Vector2 _viewDir;

    [SerializeField]
    ActorSetting actorSetting;

    private Vector3 newCameraRotation;
    private Vector3 newActorRotation;

    private void Awake()
    {
        defaultInput = new DefaultInput();
        defaultInput.Actor.Movement.performed += e => _moveDir = e.ReadValue<Vector3>();
        defaultInput.Actor.View.performed += a => _viewDir = a.ReadValue<Vector2>();
        defaultInput.Actor.Jump.performed += a => Jump();

        defaultInput.Enable();

        newCameraRotation = cameraHolder.localRotation.eulerAngles;
        newActorRotation = transform.localRotation.eulerAngles;
    }

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateView();
        CalculateMovement();
    }

    private void CalculateView()
    {
        float viewDirX;
        if (actorSetting.ViewYInverted) viewDirX = _viewDir.x;
        else viewDirX = -_viewDir.x;

        newActorRotation.y += actorSetting.viewXSensitivity * viewDirX * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(newActorRotation);

        float viewDirY;
        if (actorSetting.ViewYInverted) viewDirY = _viewDir.y;
        else viewDirY = -_viewDir.y;

        newCameraRotation.x += actorSetting.viewYSensitivity * viewDirY * Time.deltaTime;
        newCameraRotation.x = Mathf.Clamp(newCameraRotation.x, actorSetting.viewClampYMin, actorSetting.viewClampYMax);

        cameraHolder.localRotation = Quaternion.Euler(newCameraRotation);
    }

    private void FixedUpdate()
    {
        Vector3 moveDirection = transform.forward * currentInputVector.z + transform.right * currentInputVector.x;
        _rigid.AddForce(moveDirection * _moveForce, ForceMode.Acceleration);
    }

    private void CalculateMovement()
    {
        currentInputVector = Vector3.SmoothDamp(currentInputVector, _moveDir, ref smoothInputVelocity, smoothInputSpeed);

        _animator.SetFloat("MoveX", currentInputVector.x);
        _animator.SetFloat("MoveZ", currentInputVector.z);
    }

    private void Jump()
    {
        _rigid.AddForce(new Vector3(0, 20, 0), ForceMode.Impulse);
    }
}
