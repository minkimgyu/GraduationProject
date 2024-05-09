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

                                                // --> �� ���� ���� ����� ���� ��ġ ������� ����
                                                // �̸� ���� ���� �ϰԵǴ� ��� Ÿ���� ��� �ٲ���
                                                new IsCloseToTarget(blackboard.MyTransform, blackboard.CloseDistance, blackboard.CloseDistanceOffset,
                                                blackboard.ReturnTargetInSight)
                                            }
                                        ),

                                       new Retreat(blackboard.ReturnPlayer, blackboard.ReturnNodePos, blackboard.FollowPath, false)
                                    }
                                ),

                                // ���⼭�� Ÿ�ٰ��� �Ÿ��� �־��� ��� �����ϴ� ��Ŀ������ �־��
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
            // ���⼭ aimpoint ��ġ�� ���������� �ٲ��ش�.
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