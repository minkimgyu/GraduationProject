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
    public class BuildingFormationState : State
    {
        Action<Swat.MovementState> SetState;
        protected Tree _bt;

        FormationData _formationData;

        public override void OnMessageReceived(string message, FormationData data)
        {
            Debug.Log(message);
            _formationData = data;
        }

        FormationData ReturnFormationData() { return _formationData; }

        public BuildingFormationState(Action<Swat.MovementState> SetState, SwatMovementBlackboard blackboard)
        {
            this.SetState = SetState;

            _bt = new Tree();
            List<Node> _childNodes;
            _childNodes = new List<Node>()
            {
                new Sequencer
                (
                    new List<Node>()
                    {
                        new StickToPlayer(blackboard.MyTransform, blackboard.FormationRadius, blackboard.ReturnPlayer, blackboard.ReturnNodePos, 
                        blackboard.FollowPath, blackboard.View, ReturnFormationData),

                        new Selector
                        (
                            new List<Node>()
                            {
                                new Sequencer
                                (
                                    new List<Node>()
                                    {
                                        new IsCloseToTarget(blackboard.MyTransform, 9, blackboard.CloseDistanceOffset,
                                                blackboard.ReturnTargetInSight),

                                        new FaceToTarget(blackboard.MyTransform, blackboard.AimPoint, blackboard.SightPoint, blackboard.ReturnTargetInSight, blackboard.View)
                                    }
                                ),

                                 new Sequencer
                                (
                                    new List<Node>()
                                    {
                                        new ChangeAngleOfSight(blackboard.CaptureTransform, blackboard.AngleOffset, blackboard.AngleChangeAmount),

                                        new FixViewToFormation(blackboard.MyTransform, blackboard.ReturnPlayer, blackboard.View)
                                    }
                                ),

                                
                            }
                        )
                    }
                )
            };

            Node rootNode = new Selector(_childNodes);
            _bt.SetUp(rootNode);
        }

        public override void OnStateUpdate()
        {
            _bt.OnUpdate();
        }

        public override void OnHandleFreeRole()
        {
            SetState?.Invoke(Swat.MovementState.FreeRole);
        }
    }
}