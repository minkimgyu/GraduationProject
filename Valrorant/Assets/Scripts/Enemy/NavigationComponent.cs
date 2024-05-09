using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavigationComponent : DisplacementSender
{
    //public enum TargetType
    //{
    //    Point,
    //    Enemy
    //}

    //NavMeshAgent _agent;
    //Animator _animator;
    //[SerializeField] float _rotationSpeed = 120;
    //[SerializeField] float _reachDistance = 1.1f;

    //[SerializeField] float _stopDistance;

    //[SerializeField] WireDrawer _drawer;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    _agent = GetComponent<NavMeshAgent>();
    //    _animator = GetComponentInChildren<Animator>();
    //}

    //public void SetActiveRotaionUsingAgent(bool isActive)
    //{
    //    if(isActive) _agent.angularSpeed = _rotationSpeed;
    //    else _agent.angularSpeed = 0;
    //}

    //public bool NowReachToPoint()
    //{
    //    return Vector3.Distance(transform.position, _agent.destination) <= _reachDistance;
    //}

    //void SetAnimation(float xValue, float zValue)
    //{
    //    _animator.SetFloat("X", xValue);
    //    _animator.SetFloat("Z", zValue);
    //}

    //public override void RaiseDisplacementEvent() 
    //{
    //    OnDisplacementRequested?.Invoke(_agent.velocity.magnitude * _velocityLengthDecreaseRatio);
    //}

    //public void Move(Vector3 pos, TargetType targetType)
    //{
    //    if (targetType == TargetType.Point)
    //    {
    //        MoveToPosition(pos);
    //    }
    //    else
    //    {
    //        if (Vector3.Distance(transform.position, pos) <= _stopDistance)
    //        {
    //            Stop();
    //        }
    //        else if(Vector3.Distance(transform.position, pos) > _stopDistance + 1.0f)
    //        {
    //            MoveToPosition(pos);
    //        }
    //    }

    //    RaiseDisplacementEvent();
    //}

    //void Stop()
    //{
    //    _agent.SetDestination(transform.position); // 정지 상태
    //    SetAnimation(0, 0);
    //}

    //void MoveToPosition(Vector3 pos)
    //{
    //    _agent.SetDestination(pos);
    //    SetAnimation(0, 1);
    //}

    //private void OnValidate()
    //{
    //    if (_drawer == null) return;

    //    _drawer.SetNavigationValue(_stopDistance);
    //}
}
