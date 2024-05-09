using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;
using AI.ZombieFSM;

namespace BehaviorTree.Nodes
{
    public class WanderingFSM : Node
    {
        // FSM으로 처리
        public enum State
        {
            Move,
            Stop,
            Rotate
        }

        StateMachine<State> _fsm;
        public StateMachine<State> FSM { get { return _fsm; } }

        public WanderingFSM(Transform myTransform, int wanderOffset, 
            Func<Vector3, int, Vector3> ReturnNodePos, Action<Vector3, bool> FollowPath, Action<Vector3> View,  Action Stop)
        {
            _fsm = new StateMachine<State>();
            Dictionary<State, BaseState> states = new Dictionary<State, BaseState>()
            {
                {State.Move, new MoveState(ReturnNodePos, FollowPath, myTransform, wanderOffset) },
                {State.Stop, new StopState(Stop) },
                {State.Rotate, new RotateState(View, Stop, myTransform) }
            };

            _fsm.Initialize(states);
            _fsm.SetState(State.Move);
        }

        public override NodeState Evaluate()
        {
            _fsm.OnUpdate();
            return NodeState.SUCCESS;
        }
    }
}