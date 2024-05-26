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

        Func<bool> IsTargetInLargeSight;
        Func<bool> IsTargetInSmallSight;

        Action<float> ModifyLargeCaptureRadius;
        Action<float> ModifySmallCaptureRadius;

        Action<FreeRoleState.State> SetState;

        public CombatState(Action<FreeRoleState.State> SetState, SwatMovementBlackboard blackboard)
        {
            this.SetState = SetState;

            this.IsTargetInLargeSight = blackboard.IsTargetInLargeSight;
            this.IsTargetInSmallSight = blackboard.IsTargetInSmallSight;

            ModifyLargeCaptureRadius = blackboard.ModifyLargeCaptureRadius;
            ModifySmallCaptureRadius = blackboard.ModifySmallCaptureRadius;

            _bt = new Tree();
            List<Node> _childNodes;
            _childNodes = new List<Node>()
            {
                new Sequencer
                (
                    new List<Node>()
                    {
                        new FaceToTarget(blackboard.MyTransform, blackboard.AimPoint, blackboard.SightPoint, blackboard.View,
                                        blackboard.IsTargetInSmallSight, blackboard.ReturnTargetInSmallSight,
                                        blackboard.IsTargetInLargeSight, blackboard.ReturnTargetInLargeSight),

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
                                                new IsFarAwayFromPlayer(blackboard.MyTransform, blackboard.FarFromPlayerDistance,
                                                blackboard.FarFromPlayerDistanceOffset, blackboard.ReturnPlayerPos),

                                                // --> 이 경우는 가장 가까운 적의 위치 기반으로 하자
                                                // 이를 통해 후퇴를 하게되는 경우 타겟을 얘로 바꾸자
                                                new IsCloseToTarget(blackboard.MyTransform, blackboard.CloseDistance, blackboard.CloseDistanceOffset,
                                                blackboard.IsTargetInSmallSight, blackboard.ReturnTargetInSmallSight)
                                            }
                                        ),

                                        new StickToPlayer(blackboard.FormationRadius, blackboard.Offset, blackboard.OffsetChangeDuration,
                                        blackboard.ReturnPlayerPos, blackboard.FollowPath, blackboard.View, 
                                        blackboard.ReturnFormationData, blackboard.ReturnAllTargetInLargeSight),
                                    }
                                ),

                                new Stop(blackboard.Stop)
                            }
                        )
                    }
                )
            };

            Node rootNode = new Selector(_childNodes);
            _bt.SetUp(rootNode);
        }

        public override void OnStateEnter()
        {
            //Debug.Log("CombatState");
        }

        public override void OnStateExit()
        {
            _bt.OnDisable();
            // 여기서 aimpoint 위치를 정방향으로 바꿔준다.
        }

        public override void CheckStateChange()
        {
            bool isInSmallSight = IsTargetInSmallSight();
            bool isInLargeSight = IsTargetInLargeSight();

            if (isInSmallSight == true || isInLargeSight == true) return;

            // captureComponent 줄여줌
            // captureComponent 키워줌
            ModifyLargeCaptureRadius(-2f);
            ModifySmallCaptureRadius(-2f);

            SetState?.Invoke(FreeRoleState.State.Exploring);
        }

        public override void OnStateUpdate()
        {
            _bt.OnUpdate();
        }
    }
}