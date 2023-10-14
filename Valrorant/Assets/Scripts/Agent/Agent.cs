//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using BehaviorTree;

//public class Agent : Tree
//{
//    [SerializeField] private Transform actorBone;
//    [SerializeField] private Transform direction;

//    private Transform cameraHolder;

//    [SerializeField] private Transform cameraNormalTransform;
//    [SerializeField] private Transform cameraZoomTransform;

//    private Vector3 _moveDir;
//    private Rigidbody rb;

//    [SerializeField] private float maxY;
//    [SerializeField] private float rX;

//    [SerializeField] private Transform camPivot;
//    [SerializeField] private Transform cam;

//    [SerializeField]
//    private bool _isLevitating = false;
//    public bool IsLevitating { get { return _isLevitating; } }

//    [Header("Force")]
//    [SerializeField]
//    float _jumpForce = 30f;

//    [SerializeField]
//    float _moveForce = 30f;

//    [Header("View")]
//    [SerializeField]
//    private float _viewXSensitivity = 60;

//    [SerializeField]
//    private float _viewYSensitivity = 60;

//    [SerializeField]
//    private float _viewClampYMax = 40;

//    [SerializeField]
//    private float _viewClampYMin = -40;

//    [SerializeField]
//    private bool _viewXInverted = true;

//    [SerializeField]
//    private bool _viewYInverted = false;

//    WeaponHolder _weaponController;

//    public WeaponHolder WeaponController { get { return _weaponController; } }

//    Animator _animator;
//    public Animator Animator { get { return _animator; } }

//    [SerializeField]
//    AimEvent _aimEvent;

//    // Start is called before the first frame update
//    void Awake()
//    {
//        _animator = GetComponentInChildren<Animator>();
//        _weaponController = GetComponent<WeaponHolder>();
//        rb = GetComponent<Rigidbody>();


//        cameraHolder = cameraNormalTransform;

//        _aimEvent.OnAimRequested = SwitchViewMode;
//    }

//    public void SwitchViewMode(bool zoomMode)
//    {
//        if (zoomMode) cameraHolder = cameraZoomTransform;
//        else cameraHolder = cameraNormalTransform;
//    }

//    protected override void Start()
//    {
//        base.Start();

//        _weaponController.Initialize(transform, cam, _animator);
//        _weaponController.ChangeWeapon(0);
//    }

//    public void ResetDir()
//    {
//        rX = Mathf.Lerp(rX, Input.GetAxisRaw("Mouse X"), _viewXSensitivity * Time.deltaTime);
//        maxY = Mathf.Clamp(maxY - (Input.GetAxisRaw("Mouse Y") * _viewYSensitivity * Time.deltaTime), _viewClampYMin, _viewClampYMax);

//        actorBone.rotation = Quaternion.Euler(maxY, direction.eulerAngles.y, 0);

//        actorBone.Rotate(0, rX, 0, Space.World);
//        direction.Rotate(0, rX, 0, Space.World);

//        _moveDir = direction.TransformVector(new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")));
//    }

//    private void LateUpdate()
//    {
//        cam.rotation = Quaternion.Euler(maxY, direction.eulerAngles.y, 0);
//        camPivot.position = cameraHolder.position;
//    }

//    private void OnCollisionEnter(Collision collision)
//    {
//        if (collision.transform.tag == "Ground" && _isLevitating == true)
//        {
//            _isLevitating = false;
//        }
//    }

//    public void Jump()
//    {
//        _isLevitating = true;
//        rb.AddForce(new Vector3(0, _jumpForce, 0), ForceMode.Impulse);
//    }

//    private void FixedUpdate()
//    {
//        rb.AddForce(_moveDir * _moveForce, ForceMode.Force);
//    }

//    protected override Node SetUp()
//    {
//        Node root = new Selector(
//            new List<Node>
//            {
//                new Sequence(new List<Node> { new CanMainActionStart(), new MainActionStart(this) }),
//                new Sequence(new List<Node> { new CanMainActionEnd(), new MainActionEnd(this) }),

//                new Sequence(new List<Node> { new CanSubActionStart(), new SubActionStart(this) }),
//                new Sequence(new List<Node> { new CanSubActionEnd(), new SubActionEnd(this) }),

//                new Sequence(new List<Node> { new CanEquipMainWeapon(), new EquipMainWeapon(this) }),
//                new Sequence(new List<Node> { new CanEquipSubWeapon(), new EquipSubWeapon(this) }),
//                new Sequence(new List<Node> { new CanEquipKnife(), new EquipKnife(this) }),

//                new Sequence(new List<Node> { new CanReload(), new Reload(this) }),

//                new Sequence(new List<Node> { new CanJump(this), new Jump(this) }),
//                new Sequence(new List<Node> { new CanCrouch(), new Crouch(this) }),

//                new Sequence(new List<Node> { new MainActionProgress(this), new SubActionProgress(this), new Move(this)})
//            }
//        );

//        return root;
//    }
//}