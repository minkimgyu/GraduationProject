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
    public class ExploringState : State
    {
        Func<bool> IsTargetInSight;
        Action<FreeRoleState.State> SetState;

        protected Tree _bt;

        public ExploringState(Action<FreeRoleState.State> SetState, SwatMovementBlackboard blackboard)
        {
            this.SetState = SetState;

            WanderingFSM wanderingFSM = new WanderingFSM(blackboard.MyTransform, blackboard.WanderOffset,
                blackboard.ReturnNodePos, blackboard.FollowPath, blackboard.View, blackboard.Stop);

            IsFarAwayToTarget isFarAway = new IsFarAwayToTarget(blackboard.MyTransform, blackboard.FarFromPlayerDistance, blackboard.FarFromPlayerDistanceOffset,
                blackboard.ReturnPlayer, () => { wanderingFSM.FSM.SetState(WanderingFSM.State.Stop); });

            ChangeToRandomState changeState = new ChangeToRandomState(wanderingFSM.FSM.SetState);

            this.IsTargetInSight = blackboard.IsTargetInSight;

            _bt = new Tree();
            List<Node> _childNodes;
            _childNodes = new List<Node>()
            {
                new Sequencer
                (
                    new List<Node>()
                    {
                        new ChangeAngleOfSight(blackboard.CaptureTransform, blackboard.AngleOffset, blackboard.AngleChangeAmount),

                        new Selector
                        (
                            new List<Node>()
                            {
                                new Sequencer
                                (
                                    new List<Node>()
                                    {
                                        isFarAway,
                                        new Retreat(blackboard.ReturnPlayer, blackboard.ReturnNodePos, blackboard.FollowPath, true)
                                    }
                                ),

                                new Sequencer
                                (
                                    new List<Node>()
                                    {
                                        wanderingFSM,
                                        new Sequencer
                                        (
                                            new List<Node>()
                                            {
                                                new WaitForStateChange(blackboard.StateChangeDelay),
                                                changeState,
                                                // Wander에 이벤트를 보내는 방식으로 방향을 돌려준다.
                                            }
                                        )
                                        // Wander에 이벤트를 보내는 방식으로 방향을 돌려준다.
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
        }

        public override void CheckStateChange()
        {
            bool isInSight = IsTargetInSight();
            if (isInSight == false) return;

            SetState?.Invoke(FreeRoleState.State.Combat);
        }

        public override void OnStateUpdate()
        {
            _bt.OnUpdate();
        }
    }
}
