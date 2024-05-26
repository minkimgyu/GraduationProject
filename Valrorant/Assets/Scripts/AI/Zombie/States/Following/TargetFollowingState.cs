using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;
using BehaviorTree;
using BehaviorTree.Nodes;
using Tree = BehaviorTree.Tree;
using Node = BehaviorTree.Node;

namespace AI.ZombieFSM
{
    public class TargetFollowingState : State
    {
        protected Tree _bt;
        Action<Zombie.ActionState> SetState;
        Action<float> ModifyCaptureRadius;
        float _additiveCaptureRadius;

        Func<bool> IsTargetInSight;

        public TargetFollowingState(ZombieBlackboard blackboard, Action<Zombie.ActionState> SetState)
        {
            this.SetState = SetState;
            ModifyCaptureRadius = blackboard.ModifyCaptureRadius;
            IsTargetInSight = blackboard.IsTargetInSight;

            _additiveCaptureRadius = blackboard.AdditiveCaptureRadius;

            _bt = new Tree();
            List<Node> _childNodes;
            _childNodes = new List<Node>()
            {
                new Sequencer
                (
                    new List<Node>()
                    {
                        new RotateTowardTarget(blackboard.SightPoint, blackboard.ReturnTargetInSight, blackboard.View), // 정지 하는 코드 넣기

                        new Selector
                        (
                            new List<Node>()
                            {
                                 new Sequencer
                                 (
                                     new List<Node>()
                                     {
                                        new NowWithinActionRange(blackboard.MyTransform, blackboard.ReturnTargetInSight, blackboard.AttackRange, blackboard.AdditiveAttackRange),
                                        new Sequencer
                                        (
                                            new List<Node>()
                                            {
                                                new Stop(blackboard.Stop), // 정지 하는 코드 넣기
                                                new Attack(blackboard.AttackPoint, blackboard.AttackDamage, blackboard.PreAttackDelay, blackboard.DelayForNextAttack,
                                                blackboard.AttackCircleRadius, blackboard.AttackLayer, blackboard.ResetAnimatorTrigger, blackboard.IsTargetInSight, blackboard.ReturnTargetInSight, blackboard.PlaySFX),
                                                // Wander에 이벤트를 보내는 방식으로 방향을 돌려준다.
                                            }
                                        ),
                                     }
                                 ),
                                 new Follow(blackboard.ReturnTargetInSight, blackboard.FollowPath)
                            }
                        )
                    }
                )
            };

            Node rootNode = new Selector(_childNodes);
            _bt.SetUp(rootNode);
        }

        public override void CheckStateChange()
        {
            bool isInSight = IsTargetInSight();
            if (isInSight == true) return;

            SetState?.Invoke(Zombie.ActionState.Idle);
        }

        public override void OnStateEnter()
        {
            ModifyCaptureRadius?.Invoke(_additiveCaptureRadius);
            //Debug.Log("FollowState");
        }

        public override void OnStateUpdate()
        {
            _bt.OnUpdate();
        }

        // 여기서 delay 리셋해보자
        public override void OnStateExit()
        {
            ModifyCaptureRadius?.Invoke(-_additiveCaptureRadius);
            _bt.OnDisable();
        }
    }
}