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
        Action<Helper.MovementState> SetState;
        protected Tree _bt;

        public BuildingFormationState(Action<Helper.MovementState> SetState, SwatMovementBlackboard blackboard)
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
                        new StickToPlayer(blackboard.FormationRadius, blackboard.Offset, blackboard.OffsetChangeDuration, 
                        blackboard.ReturnPlayerPos, blackboard.ReturnNodePos, blackboard.FollowPath, blackboard.View, 
                        blackboard.ReturnFormationData, blackboard.ReturnAllTargetInLargeSight),

                        new Selector
                        (
                            new List<Node>()
                            {
                                new Sequencer
                                (
                                    new List<Node>()
                                    {
                                        new IsCloseToTarget(blackboard.MyTransform, blackboard.FarFromTargetDistance, blackboard.FarFromTargetDistanceOffset,
                                                blackboard.IsTargetInLargeSight, blackboard.ReturnTargetInLargeSight),

                                        new FaceToTarget(blackboard.MyTransform, blackboard.AimPoint, blackboard.SightPoint, blackboard.View,
                                        blackboard.IsTargetInSmallSight, blackboard.ReturnTargetInSmallSight,
                                        blackboard.IsTargetInLargeSight, blackboard.ReturnTargetInLargeSight)
                                    }
                                ),

                                new ChangeAngleOfSight(blackboard.CaptureTransform, blackboard.AngleOffset, blackboard.AngleChangeAmount),
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
            SetState?.Invoke(Helper.MovementState.FreeRole);
        }
    }
}