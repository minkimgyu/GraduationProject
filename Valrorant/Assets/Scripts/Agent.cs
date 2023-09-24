using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : Tree
{
    [SerializeField] private Transform player;
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

    [SerializeField]
    BaseWeapon weapon;

    public BaseWeapon Weapon { get { return weapon; } }

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        weapon.Initialize(transform, cam);
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
                new Sequence(new List<Node> { new CanShoot(this), new Shoot(this) }),
                new Sequence(new List<Node> { new CanJump(this), new Jump(this) }),
                new Sequence(new List<Node> { new CanCrouch(), new Crouch(this) }),
                new Sequence(new List<Node> { new Move(this) })
            }
        );

        return root;
    }
}

public class CanShoot : Node
{
    Agent loadAgent;

    public CanShoot(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        if (Input.GetMouseButtonDown(0)) _state = NodeState.SUCCESS;
        else _state = NodeState.FAILURE;

        return _state;
    }
}

public class Shoot : Node
{
    Agent loadAgent;

    public Shoot(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        // Shoot Àû¿ë
        loadAgent.Weapon.Action();

        _state = NodeState.SUCCESS;
        return _state;
    }
}

public class CanJump : Node
{
    Agent loadAgent;

    public CanJump(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        if (Input.GetKeyDown(KeyCode.Space) && loadAgent.IsLevitating == false) _state = NodeState.SUCCESS;
        else _state = NodeState.FAILURE;

        return _state;
    }
}

public class Jump : Node
{
    Agent loadAgent;

    public Jump(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        loadAgent.Jump();

        _state = NodeState.SUCCESS;
        return _state;
    }
}

public class CanCrouch : Node
{
    public override NodeState Evaluate()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.LeftControl))
        {
            _state = NodeState.SUCCESS;
        }
        else _state = NodeState.FAILURE;

        return _state;
    }
}

public class Crouch : Node
{
    bool nowCrouch = false;

    Agent loadAgent;

    public Crouch(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        nowCrouch = !nowCrouch;
        //loadAgent.Crouch(nowCrouch);

        _state = NodeState.SUCCESS;
        return _state;
    }
}

public class Move : Node
{
    Agent loadAgent;

    public Move(Agent agent) : base()
    {
        loadAgent = agent;
    }

    public override NodeState Evaluate()
    {
        loadAgent.ResetDir();

        _state = NodeState.SUCCESS;
        return _state;
    }
}

