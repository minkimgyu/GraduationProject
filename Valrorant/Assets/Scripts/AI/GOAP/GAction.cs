using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public abstract class GAction : MonoBehaviour
{
    public string _actionName = "Action";
    public float _cost = 1.0f;
    public GameObject _target;
    public GameObject _targetTag;
    public float _duration = 0;
    public WorldState[] _storedPreConditions;
    public WorldState[] _storedAfterEffects;

    public Dictionary<string, int> _preConditions;
    public Dictionary<string, int> _afterEffects;

    public WorldStates agentBeliefs;

    public bool running = false;

    public Action<Vector3, Vector3> FindPath;
    public Action<Vector3, bool> RouteTracking;

    public GAction()
    {
        _preConditions = new Dictionary<string, int>();
        _afterEffects = new Dictionary<string, int>();
    }

    private void Start()
    {
        if (_storedPreConditions != null)
            foreach (WorldState state in _storedPreConditions)
            {
                _preConditions.Add(state.key, state.value);
            }

        if (_storedAfterEffects != null)
            foreach (WorldState state in _storedAfterEffects)
            {
                _afterEffects.Add(state.key, state.value);
            }
    }

    public bool IsAchievable()
    {
        return true;
    }

    public bool IsAchievableGiven(Dictionary<string, int> conditions)
    {
        foreach (KeyValuePair<string, int> condition in _preConditions)
        {
            if (!conditions.ContainsKey(condition.Key)) return false;
        }
        return true;
    }

    public abstract bool PrePerform();
    public abstract bool PosePerform();
}
