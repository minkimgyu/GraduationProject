using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;
using BehaviorTree;
using BehaviorTree.Nodes;
using Tree = BehaviorTree.Tree;
using Node = BehaviorTree.Node;

namespace AI.FSM
{
    public class IdleState : State
    {
        protected Tree _bt;
        Action<Zombie.ActionState> SetState;
        Func<bool> IsTargetInSight;

        public IdleState(ZombieBlackboard blackboard, Action<Zombie.ActionState> SetState)
        {
            this.SetState = SetState;
            IsTargetInSight = blackboard.IsTargetInSight;

            WanderingFSM wanderingFSM = new WanderingFSM(blackboard);
            ChangeToRandomState changeState = new ChangeToRandomState(wanderingFSM.FSM.SetState);

            _bt = new Tree();
            List<Node> _childNodes;
            _childNodes = new List<Node>()
            {
                new Sequencer
                (
                    new List<Node>()
                    {
                        new ChangeAngleOfSight(blackboard.CaptureTransform, blackboard.AngleOffset, blackboard.AngleChangeAmount),
                        wanderingFSM,

                        new Sequencer
                        (
                            new List<Node>()
                            {
                                new WaitForStateChange(blackboard.StateChangeDelay),
                                changeState,
                                // Wander�� �̺�Ʈ�� ������ ������� ������ �����ش�.
                            }
                        ),
                    }
                )
            };

            Node rootNode = new Selector(_childNodes);
            _bt.SetUp(rootNode);
        }

        // �̺�Ʈ�� �̿��ؼ� �ޱ�
        // ���� �Ҹ��� �����ߴٸ� ���� Ż��
        public override void OnNoiseReceived()
        {
            SetState?.Invoke(Zombie.ActionState.NoiseTracking);
        }

        public override void CheckStateChange()
        {
            bool isInSight = IsTargetInSight();
            if (isInSight == false) return;

            SetState?.Invoke(Zombie.ActionState.TargetFollowing);
        }

        public override void OnStateEnter()
        {
            //Debug.Log("IdleState");
        }

        public override void OnStateUpdate()
        {
            _bt.OnUpdate();
        }

        // ���⼭ delay �����غ���
        public override void OnStateExit()
        {
            _bt.OnDisable();
        }
    }
}