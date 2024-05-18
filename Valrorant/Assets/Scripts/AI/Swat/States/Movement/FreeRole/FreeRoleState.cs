using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree.Nodes;
using BehaviorTree;
using FSM;
using System;
using Tree = BehaviorTree.Tree;
using Node = BehaviorTree.Node;

namespace AI.SwatFSM
{
    public class FreeRoleState : State
    {
        public enum State
        {
            Exploring,
            Combat
        }

        protected Tree _bt;
        Action<Helper.MovementState> SetState;

        StateMachine<State> _fsm = new StateMachine<State>();

        public FreeRoleState(Action<Helper.MovementState> SetState, SwatMovementBlackboard blackboard)
        {
            this.SetState = SetState;

            _fsm.Initialize(
               new Dictionary<State, BaseState>
               {
                    {State.Exploring, new ExploringState((state) => _fsm.SetState(state), blackboard) },
                    {State.Combat, new CombatState((state) => _fsm.SetState(state), blackboard) },
               }
            );
            _fsm.SetState(State.Exploring);
        }

        public override void OnStateUpdate()
        {
            _fsm.OnUpdate();
        }

        public override void OnHandleBuildFormation()
        {
            SetState?.Invoke(Helper.MovementState.BuildingFormation);
        }
    }
}