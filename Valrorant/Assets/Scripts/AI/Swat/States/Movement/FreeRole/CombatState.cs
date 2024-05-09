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
    public class CombatState : State
    {
        protected Tree _bt;

        Func<bool> IsTargetInSight;
        Action<FreeRoleState.State> SetState;

        public CombatState(Action<FreeRoleState.State> SetState, SwatMovementBlackboard blackboard)
        {
            this.SetState = SetState;
            this.IsTargetInSight = blackboard.IsTargetInSight;

            _bt = new Tree();
            List<Node> _childNodes;
            _childNodes = new List<Node>()
            {
                new Sequencer
                (
                    new List<Node>()
                    {
                        new FaceToTarget(blackboard.MyTransform, blackboard.AimPoint, blackboard.SightPoint, blackboard.ReturnTargetInSight, blackboard.View),

                        new Selector
                        (
                            new List<Node>()
                            {
                                new Sequencer
                                (
                                    new List<Node>()
                                    {
                                        new Selector
                                        (
                                            new List<Node>()
                                            {
                                                new IsFarAwayToTarget(blackboard.MyTransform, blackboard.FarFromPlayerDistance,
                                                blackboard.FarFromPlayerDistanceOffset, blackboard.ReturnPlayer),

                                                // --> 이 경우는 가장 가까운 적의 위치 기반으로 하자
                                                // 이를 통해 후퇴를 하게되는 경우 타겟을 얘로 바꾸자
                                                new IsCloseToTarget(blackboard.MyTransform, blackboard.CloseDistance, blackboard.CloseDistanceOffset,
                                                blackboard.ReturnTargetInSight)
                                            }
                                        ),

                                       new Retreat(blackboard.ReturnPlayer, blackboard.ReturnNodePos, blackboard.FollowPath, false)
                                    }
                                ),

                                // 여기서는 타겟과의 거리가 멀어질 경우 추적하는 메커니즘을 넣어보자
                                new Sequencer
                                (
                                    new List<Node>()
                                    {
                                        new IsFarAwayToTarget(blackboard.MyTransform, blackboard.FarFromTargetDistance,
                                                blackboard.FarFromTargetDistanceOffset, blackboard.ReturnTargetInSight),

                                       new FollowTarget(blackboard.ReturnTargetInSight, blackboard.ReturnNodePos, blackboard.FollowPath)
                                    }
                                )
                            }
                        )
                    }
                )
            };

            Node rootNode = new Selector(_childNodes);
            _bt.SetUp(rootNode);
        }

        public override void OnStateExit()
        {

            _bt.OnDisable();
            // 여기서 aimpoint 위치를 정방향으로 바꿔준다.
        }

        public override void CheckStateChange()
        {
            bool isInSight = IsTargetInSight();
            if (isInSight == true) return;

            SetState?.Invoke(FreeRoleState.State.Exploring);
        }

        public override void OnStateUpdate()
        {
            _bt.OnUpdate();
        }
    }
}