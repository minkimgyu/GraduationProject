//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using BehaviorTree;
//using System;
//using Random = UnityEngine.Random;

//public class Enemy : MonoBehaviour, IDamageable
//{
//    Animator _animator;
//    //Tree _bt;

//    [SerializeField] Transform[] _points;
//    CaptureComponent _captureComponent;
//    NavigationComponent _navigationComponent;
//    BattleComponent _attackComponent;
//    AimComponent _aimComponent;

//    public Func<Vector3, Vector3> OnAmmoBoxPosRequested;

//    public Action OnDeathRequested;

//    [SerializeField] float _maxHp;
//    public float HP { get; set; }

//    public Vector3 GetFowardVector()
//    {
//        return transform.forward;
//    }

//    public void GetDamage(float damage)
//    {
//        HP -= damage;
//        if(HP <= 0)
//        {
//            // StageManager에 메시지 보내기
//            //Destroy(gameObject);
//            Debug.Log(HP);
//            OnDeathRequested?.Invoke();
//        }
//    }

//    private void Start()
//    {
//        HP = _maxHp;
//        _animator = GetComponentInChildren<Animator>();
//        _captureComponent = GetComponentInChildren<CaptureComponent>();
//        _navigationComponent = GetComponent<NavigationComponent>();
//        _attackComponent = GetComponent<BattleComponent>();

//        _aimComponent = GetComponent<AimComponent>();

//        GameObject ammoBoxSpawner = GameObject.FindWithTag("AmmoBoxSpawner");
//        if (ammoBoxSpawner == null) return;

//        AmmoBoxSpawner spawner = ammoBoxSpawner.GetComponent<AmmoBoxSpawner>();
//        if (spawner == null) return;

//        OnAmmoBoxPosRequested += spawner.ReturnClosestPoint;

//        //_bt = new Tree();
//        SetUpBT();


//        GameObject go = GameObject.FindWithTag("StageManager");
//        if (go == null) return;

//        StageManager stageManager = go.GetComponent<StageManager>();
//        if (stageManager == null) return;

//        OnDeathRequested += stageManager.OnEnemyKillRequested;
//    }

//    void SetUpBT()
//    {
//        //List<Node> _childNodes = new List<Node>() {

//        //new IsAmmoEmpty(_attackComponent, 
//        //    new GoToClosestAmmoPosition(_navigationComponent, OnAmmoBoxPosRequested, transform)
//        //),

//        //new FindEnemy(_captureComponent, 

//        //new Sequence(new List<Node>(){ 
//        //    // 여기에 추적가능한지 체크 한번 해주기
//        //    new FaceToEnemy(_captureComponent, _aimComponent, transform),
//        //    new FollowEnemy(_captureComponent, _navigationComponent),

//        //    new Selector(new List<Node>{ 
//        //        new WaitForReloadFinish(_attackComponent),
//        //        new NeedToReload(_attackComponent, new Reload(_attackComponent)),

//        //        new WaitForNextAttack(_attackComponent),
//        //        new CanAttack(_attackComponent, new Attack(_attackComponent))
//        //    }),
//        //})), 

//        //new Patrol(_navigationComponent, _animator, _points) };

//        //Node rootNode = new Selector(_childNodes);

//        //_bt.SetUp(rootNode);
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        //_bt.OnUpdate();
//    }
//}

//public class IsAmmoEmpty : IFNode
//{
//    BattleComponent _battleComponent;

//    public IsAmmoEmpty(BattleComponent battleComponent, Node childNode) : base(childNode)
//    {
//        _battleComponent = battleComponent;
//    }

//    protected override bool CheckCondition()
//    {
//        return _battleComponent.NowNeedToRefillAmmo();
//    }
//}

//public class GoToClosestAmmoPosition : Node
//{
//    NavigationComponent _navigationComponent;
//    Func<Vector3, Vector3> OnAmmoBoxPosRequested;
//    Transform _characterTr;

//    public GoToClosestAmmoPosition(NavigationComponent navigationComponent, Func<Vector3, Vector3> OnAmmoBoxPosRequested, Transform characterTr) : base()
//    {
//        _navigationComponent = navigationComponent;
//        this.OnAmmoBoxPosRequested = OnAmmoBoxPosRequested;
//        _characterTr = characterTr;
//    }

//    public override NodeState Evaluate()
//    {
//        Vector3 targetPos = OnAmmoBoxPosRequested(_characterTr.position);
//        //_navigationComponent.Move(targetPos, TargetType.Point);

//        return NodeState.SUCCESS;
//    }
//}

//public class WaitForReloadFinish : Node
//{
//    BattleComponent _battleComponent;

//    public WaitForReloadFinish(BattleComponent battleComponent) : base()
//    {
//        _battleComponent = battleComponent;
//    }

//    public override NodeState Evaluate()
//    {
//        if (_battleComponent.IsReloadFinish() == false && _battleComponent.IsReloadRunning() == true)
//        {
//            return NodeState.RUNNING;
//        }
//        else
//        {
//            if(_battleComponent.IsReloadFinish() == true)
//            {
//                _battleComponent.ResetReload();
//            }

//            return NodeState.FAILURE;
//        }
//    }
//}

//public class NeedToReload : IFNode
//{
//    BattleComponent _attackComponent;

//    public NeedToReload(BattleComponent attackComponent, Node childNode) : base(childNode)
//    {
//        _attackComponent = attackComponent;
//    }

//    protected override bool CheckCondition()
//    {
//        return _attackComponent.NeedToReload();
//    }
//}

//public class Reload : Node
//{
//    BattleComponent _battleComponent;

//    public Reload(BattleComponent battleComponent) : base()
//    {
//        _battleComponent = battleComponent;
//    }

//    public override NodeState Evaluate()
//    {
//        _battleComponent.Reload();
//        return NodeState.SUCCESS;
//    }
//}

//public class WaitForNextAttack : Node
//{
//    BattleComponent _battleComponent;
//    Timer _delayTimer;
//    float _delayDuration = 0.5f;

//    public WaitForNextAttack(BattleComponent battleComponent) : base()
//    {
//        _battleComponent = battleComponent;
//        _delayTimer = new Timer();
//    }

//    public override NodeState Evaluate()
//    {
//        _delayTimer.Update();

//        if (_battleComponent.IsAttackFinish())
//        {
//            _delayTimer.Start(_delayDuration);
//        }

//        if(_delayTimer.IsRunning)
//        {
//            return NodeState.RUNNING;
//        }

//        if(_delayTimer.IsFinish)
//        {
//            _delayTimer.Reset();
//            _battleComponent.ResetAttack();
//            return NodeState.FAILURE;
//        }

//        return NodeState.FAILURE;
//    }
//}

//public class CanAttack : IFNode
//{
//    BattleComponent _battleComponent;

//    public CanAttack(BattleComponent battleComponent, Node childNode) : base(childNode)
//    {
//        _battleComponent = battleComponent;
//    }

//    protected override bool CheckCondition()
//    {
//        return _battleComponent.CanAttack();
//    }
//}

//public class Attack : Node
//{
//    BattleComponent _battleComponent;

//    public Attack(BattleComponent battleComponent) : base()
//    {
//        _battleComponent = battleComponent;
//    }

//    public override NodeState Evaluate()
//    {
//        _battleComponent.Attack();
//        return NodeState.SUCCESS;
//    }
//}

//public class FindEnemy : IFNode
//{
//    CaptureComponent _captureComponent;

//    public FindEnemy(CaptureComponent captureComponent, Node childNode) : base(childNode)
//    {
//        _captureComponent = captureComponent;
//    }

//    protected override bool CheckCondition() 
//    {
//        return _captureComponent.IsTargetInSight();
//    }
//}

//public class FaceToEnemy : Node
//{
//    CaptureComponent _captureComponent;
//    AimComponent _aimComponent;
//    Transform _characterTr;
//    Vector3 _dir;

//    public FaceToEnemy(CaptureComponent captureComponent, AimComponent aimComponent, Transform characterTr) : base()
//    {
//        _captureComponent = captureComponent;
//        _aimComponent = aimComponent;
//        _characterTr = characterTr;
//    }

//    void FacingToTarget(Transform target)
//    {
//        _dir.Set(target.position.x, _characterTr.position.y, target.position.z);

//        _characterTr.LookAt(_dir);
//        _aimComponent.AimToPoint(target);
//        _aimComponent.ResetCamera();
//    }

//    public override NodeState Evaluate()
//    {
//        FacingToTarget(_captureComponent.CapturedEnemy);

//        return NodeState.SUCCESS;
//    }
//}

//public class FollowEnemy : Node
//{
//    CaptureComponent _captureComponent;
//    NavigationComponent _navigationComponent;

//    public FollowEnemy(CaptureComponent captureComponent, NavigationComponent navigationComponent) : base()
//    {
//        _captureComponent = captureComponent;
//        _navigationComponent = navigationComponent;
//    }

//    public override NodeState Evaluate()
//    {
//        //_navigationComponent.Move(_captureComponent.CapturedEnemy.position, TargetType.Enemy);

//        return NodeState.SUCCESS;
//    }
//}

//public class Patrol : Node
//{
//    Animator _animator;
//    NavigationComponent _navigationComponent;
//    Transform[] _points;
//    int _nowIndex = -1;

//    void ResetDestinationIndex()
//    {
//        bool isIndexAvailable = false;

//        while (isIndexAvailable == false)
//        {
//            int tmpIndex = Random.Range(0, _points.Length);
//            if (tmpIndex != _nowIndex)
//            {
//                isIndexAvailable = true;
//                _nowIndex = tmpIndex;
//            }
//        }
//    }

//    public Patrol(NavigationComponent navigationComponent, Animator animator, Transform[] points) : base()
//    {
//        _navigationComponent = navigationComponent;
//        _animator = animator;
//        _points = points;

//        ResetDestinationIndex();
//    }

//    public override NodeState Evaluate()
//    {
//        if (_navigationComponent.NowReachToPoint())
//        {
//            ResetDestinationIndex();
//        }

//        //_navigationComponent.Move(_points[_nowIndex].position, TargetType.Point);
//        return NodeState.SUCCESS;
//    }
//}
