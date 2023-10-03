using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class Agent : Tree
{
    [SerializeField] private Transform actorBone;
    [SerializeField] private Transform direction;

    [SerializeField] private Transform cameraHolder;
    private Vector3 _moveDir;
    private Rigidbody rb;

    [SerializeField] private float maxY;
    [SerializeField] private float rX;

    [SerializeField] private Transform camPivot;
    [SerializeField] private Transform cam;


    [SerializeField]
    private bool _isLevitating = false;
    public bool IsLevitating { get { return _isLevitating; } }

    [Header("Force")]
    [SerializeField]
    float _jumpForce = 30f;

    [SerializeField]
    float _moveForce = 30f;

    [Header("View")]
    [SerializeField]
    private float _viewXSensitivity = 60;

    [SerializeField]
    private float _viewYSensitivity = 60;

    [SerializeField]
    private float _viewClampYMax = 40;

    [SerializeField]
    private float _viewClampYMin = -40;

    [SerializeField]
    private bool _viewXInverted = true;

    [SerializeField]
    private bool _viewYInverted = false;

    WeaponController _weaponController;

    public WeaponController WeaponController { get { return _weaponController; } }

    Animator _animator;
    public Animator Animator { get { return _animator; } }

    // Start is called before the first frame update
    void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _weaponController = GetComponent<WeaponController>();
        rb = GetComponent<Rigidbody>();

        _weaponController.Initialize(transform, cam, _animator);
    }

    protected override void Start()
    {
        base.Start();
        _weaponController.ChangeWeapon(0);
    }

    public void ResetDir()
    {
        rX = Mathf.Lerp(rX, Input.GetAxisRaw("Mouse X"), _viewXSensitivity * Time.deltaTime);
        maxY = Mathf.Clamp(maxY - (Input.GetAxisRaw("Mouse Y") * _viewYSensitivity * Time.deltaTime), _viewClampYMin, _viewClampYMax);

        actorBone.rotation = Quaternion.Euler(maxY, direction.eulerAngles.y, 0);

        actorBone.Rotate(0, rX, 0, Space.World);
        direction.Rotate(0, rX, 0, Space.World);

        _moveDir = direction.TransformVector(new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")));
    }

    private void LateUpdate()
    {
        actorBone.rotation = cam.rotation = Quaternion.Euler(maxY, direction.eulerAngles.y, 0);
        camPivot.position = cameraHolder.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Ground" && _isLevitating == true)
        {
            _isLevitating = false;
        }
    }

    public void Jump()
    {
        _isLevitating = true;
        rb.AddForce(new Vector3(0, _jumpForce, 0), ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        rb.AddForce(_moveDir * _moveForce, ForceMode.Force);
    }

    protected override Node SetUp()
    {
        Node root = new Selector(
            new List<Node>
            {
                new Sequence(new List<Node> { new CanPlayMainAction(this), new PlayMainAction(this) }),
                new Sequence(new List<Node> { new CanStopMainAction(this), new StopMainAction(this) }),

                new Sequence(new List<Node> { new CanPlaySubAction(this), new PlaySubAction(this) }),
                new Sequence(new List<Node> { new CanStopSubAction(this), new StopSubAction(this) }),


                new Sequence(new List<Node> { new CanEquipMainWeapon(this), new EquipMainWeapon(this) }),
                new Sequence(new List<Node> { new CanEquipSubWeapon(this), new EquipSubWeapon(this) }),
                new Sequence(new List<Node> { new CanEquipKnife(this), new EquipKnife(this) }),

                new Sequence(new List<Node> { new CanReload(this), new Reload(this) }),

                new Sequence(new List<Node> { new CanJump(this), new Jump(this) }),
                new Sequence(new List<Node> { new CanCrouch(), new Crouch(this) }),
                new Sequence(new List<Node> { new   new Move(this) })
            }
        );

        return root;
    }
}