using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SubGoal
{
    public Dictionary<string, int> sGoal;
    public bool removable;

    public SubGoal(string key, int value, bool removable)
    {
        sGoal = new Dictionary<string, int>();
        sGoal.Add(key, value);
        this.removable = removable;
    }
}

public class GAgent : MonoBehaviour
{
    public List<GAction> _actions = new List<GAction>();
    public Dictionary<SubGoal, int> _goals = new Dictionary<SubGoal, int>();

    GPlanner _planner;
    Queue<GAction> _actionQueue;
    public GAction _currentAction;
    SubGoal _currentGoal;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        GAction[] actions = GetComponents<GAction>();
        _actions = actions.ToList();
    }

    void CompleteAction()
    {
        _currentAction.running = false;
        _currentAction.PosePerform();
    }

    private void LateUpdate()
    {
        if(_currentAction != null && _currentAction.running)
        {
            // 동작 넣기

            // 이동 동작 넣어서 테스트 해보자

            // 동작이 완료된 경우 실행
            CompleteAction();
        }

        if(_planner == null || _actionQueue == null)
        {
            _planner = new GPlanner();
            var sortedGoals = from entry in _goals orderby entry.Value descending select entry;

            foreach (KeyValuePair<SubGoal, int> goal in sortedGoals)
            {
                _actionQueue = _planner.Plan(_actions, goal.Key.sGoal, null);
                if(_actionQueue != null)
                {
                    _currentGoal = goal.Key;
                    break;
                }
            }
        }

        if(_actionQueue != null && _actionQueue.Count == 0)
        {
            if(_currentGoal.removable)
            {
                _goals.Remove(_currentGoal);
            }
            _planner = null;
        }

        if(_actionQueue != null && _actionQueue.Count > 0)
        {
            _currentAction = _actionQueue.Dequeue();
            if(_currentAction.PrePerform())
            {
                // 동작 실행
            }
            else
            {
                _actionQueue = null;
            }
        }
    }
}
