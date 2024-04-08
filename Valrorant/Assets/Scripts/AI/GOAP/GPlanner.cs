using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPlanner
{
    public class Node
    {
        public Node parent;
        public float cost;
        public Dictionary<string, int> state;
        public GAction action;

        public Node(Node parent, float cost, Dictionary<string, int> allStates, GAction action)
        {
            this.parent = parent;
            this.cost = cost;
            this.state = new Dictionary<string, int>(allStates); // บนป็บป
            this.action = action;
        }
    }

    public Queue<GAction> Plan(List<GAction> actions, Dictionary<string, int> goal, WorldStates states)
    {
        List<GAction> usableActions = new List<GAction>();
        foreach (GAction action in actions)
        {
            if (action.IsAchievable()) usableActions.Add(action);
        }

        List<Node> leaves = new List<Node>();
        Node start = new Node(null, 0, GWorld.Instance.World.GetStates(), null);

        bool success = BuildGraph(start, leaves, usableActions, goal);
        if(!success)
        {
            Debug.Log("NO PLAN");
            return null;
        }

        Node cheapest = null;
        foreach (Node leaf in leaves)
        {
            if (cheapest == null) cheapest = leaf;
            else
            {
                if(leaf.cost < cheapest.cost)
                {
                    cheapest = leaf;
                }
            }
        }

        List<GAction> result = new List<GAction>();
        Node node = cheapest;
        while (node != null)
        {
            if(node.action != null)
            {
                result.Insert(0, node.action);
            }
            node = node.parent;
        }

        Queue<GAction> queue = new Queue<GAction>();
        foreach (GAction action in result)
        {
            queue.Enqueue(action);
        }

        Debug.Log("The Plan is: ");
        foreach (GAction action in queue)
        {
            Debug.Log("Q" + action._actionName);
        }

        return queue;
    }

    private bool BuildGraph(Node parent, List<Node> leaves, List<GAction> usuableActions, Dictionary<string, int> goal)
    {
        bool foundPath = false;
        foreach(GAction action in usuableActions)
        {
            if(action.IsAchievableGiven(parent.state))
            {
                Dictionary<string, int> currentState = new Dictionary<string, int>(parent.state);
                foreach (KeyValuePair<string, int> effect in action._afterEffects)
                {
                    if(!currentState.ContainsKey(effect.Key))
                        currentState.Add(effect.Key, effect.Value);
                }

                Node node = new Node(parent, parent.cost + action._cost, currentState, action);

                if(GoalAchieved(goal, currentState))
                {
                    leaves.Add(node);
                    foundPath = true;
                }
                else
                {
                    List<GAction> subset = ActionSubset(usuableActions, action);
                    bool found = BuildGraph(node, leaves, subset, goal);
                    if (found == true) foundPath = true;
                }
            }
        }

        return foundPath;
    }

    private bool GoalAchieved(Dictionary<string, int> goal, Dictionary<string, int> currentState)
    {
        foreach (KeyValuePair<string, int> g in goal)
        {
            if (!currentState.ContainsKey(g.Key)) return false;
        }
        return true;
    }

    private List<GAction> ActionSubset(List<GAction> actions, GAction actionToRemove)
    {
        List<GAction> subset = new List<GAction>();
        foreach (GAction action in actions)
        {
            if (action.Equals(actionToRemove)) subset.Add(action);
        }
        return subset;
    }
}
