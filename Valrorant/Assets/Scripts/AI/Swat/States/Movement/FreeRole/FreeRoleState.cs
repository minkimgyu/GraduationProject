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
        Action<Swat.MovementState, string, FormationData> SetState;

        StateMachine<State> _fsm = new StateMachine<State>();

        public FreeRoleState(Action<Swat.MovementState, string, FormationData> SetState, SwatMovementBlackboard blackboard)
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

        public override void OnHandleBuildFormation(FormationData data)
        {
            SetState?.Invoke(Swat.MovementState.BuildingFormation, "BuildingFormation", data);
        }
    }
}