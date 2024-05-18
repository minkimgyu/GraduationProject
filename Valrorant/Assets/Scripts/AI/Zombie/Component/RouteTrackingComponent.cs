using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RouteTrackingComponent : MonoBehaviour
{
    Action<Vector3> Move;
    Action Stop;

    Action<Vector3> View;

    Func<Vector3, Vector3, List<Vector3>, List<Vector3>> FindPath;

    List<Vector3> _paths = new List<Vector3>();
    int _pathIndex = 0;

    StopwatchTimer _stopwatchTimer;
    float _pathFindDelay = 0.3f;


    bool _isFollowingFinish = false;
    Vector3 _storedTargetPos; // 저장된 타겟의 위치


    public void Initialize(float pathFindDelay, Action<Vector3> Move, Action Stop, Action<Vector3> View, 
        Func<Vector3, Vector3, List<Vector3>, List<Vector3>> FindPath)
    {
        _stopwatchTimer = new StopwatchTimer();

        _pathFindDelay = pathFindDelay;
        this.Move = Move;
        this.Stop = Stop;
        this.View = View;
        this.FindPath = FindPath;
    }

    void ResetPath()
    {
        if (_paths == null || _paths.Count == 0) return;

        _paths.Clear();
        _pathIndex = 1;
    }

    void CreatePath(Vector3 targetPos, List<Vector3> ignorePos)
    {
        if (_storedTargetPos == targetPos || transform.position == targetPos)
        {
            Stop?.Invoke();
            // 이 경우 MoveAnimation을 Stop으로 바꿔준다.
            return;
        }
        _storedTargetPos = targetPos;

        if (_stopwatchTimer.CurrentState == StopwatchTimer.State.Running) return;
       
        if (_stopwatchTimer.CurrentState == StopwatchTimer.State.Finish)
        {
            _stopwatchTimer.Reset();
            _stopwatchTimer.Start(_pathFindDelay);
        }

        _isFollowingFinish = false; // 다시 길찾기를 시작할 때
        ResetPath();
        _paths = FindPath(transform.position, targetPos, ignorePos);
    }

    public bool IsFollowingFinish() { return _isFollowingFinish; }

    public void StopFollow()
    {
        _isFollowingFinish = false; // 다시 길찾기를 시작할 때
        ResetPath();
    }

    public void FollowPath(Vector3 targetPos, List<Vector3> ignorePos = null, bool lookPath = false)
    {
        CreatePath(targetPos, ignorePos);
        if (_isFollowingFinish == true) return;

        if (_paths == null) return;
        if (_paths.Count == 0 || _pathIndex > _paths.Count - 1) return; // CreatePath가 작동하지 않은 경우 작동 X

        Vector3 nextPathPos = new Vector3(_paths[_pathIndex].x, transform.position.y, _paths[_pathIndex].z);

        if(lookPath) View?.Invoke((_paths[_pathIndex] - transform.position).normalized);

        Vector3 dir = (nextPathPos - transform.position).normalized;
        Move?.Invoke(dir);

        float distance = Vector3.Distance(transform.position, nextPathPos);
        if (distance < 0.1f)
        {
            if (_paths.Count - 1 <= _pathIndex)
            {
                ResetPath();
                Stop?.Invoke();
                _isFollowingFinish = true;
            }
            else _pathIndex++;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_paths == null || _paths.Count == 0) return;

        for (int i = 0; i < _paths.Count; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(_paths[i], Vector3.one);
        }
    }
}
