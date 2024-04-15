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

        public WanderingFSM(ZombieBlackboard blackboard)
        {
            _fsm = new StateMachine<State>();
            Dictionary<State, BaseState> states = new Dictionary<State, BaseState>()
            {
                {State.Move, new MoveState(blackboard.ReturnNodePos, blackboard.FollowPath, blackboard.MyTrasform, blackboard.WanderOffset) },
                {State.Stop, new StopState(blackboard.Stop) },
                {State.Rotate, new RotateState(blackboard.View, blackboard.Stop, blackboard.MyTrasform) }
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