using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using BehaviorTree.Nodes;
using Tree = BehaviorTree.Tree;
using Node = BehaviorTree.Node;
using System;
using FSM;
using AI.SwatFSM;

public class BattleFSM : MonoBehaviour
{
    public enum State
    {
        Idle,
        Attack,
        Reload
    }

    StateMachine<State> _battleFSM;
    // ���⿡ ���� ���� ������ֱ�

    [SerializeField] BaseWeapon _equipedWeapon;
    // ����, ��� ��� �ϼ���Ű��
    // ȸ�� ��ɵ� �߰��غ���

    [SerializeField] Animator _animator;

    public void Initialize(SwatBlackboard bloackboard)
    {
        _equipedWeapon.Initialize(gameObject, gameObject, transform, _animator);

        _battleFSM = new StateMachine<State>();

        _battleFSM.Initialize(
            new Dictionary<State, BaseState>()
            {
                {State.Idle, new IdleState(bloackboard, SetState) },
                {
                    State.Attack, new AttackState(bloackboard, SetState,
                    _equipedWeapon.OnLeftClickStart, 
                    _equipedWeapon.OnLeftClickProgress, 
                    _equipedWeapon.OnLeftClickEnd) 
                }
            }
        );
        _battleFSM.SetState(State.Idle);
    }

    void Fire()
    {
        _equipedWeapon.OnLeftClickStart();
        _equipedWeapon.OnLeftClickProgress();
        _equipedWeapon.OnLeftClickEnd();
    }

    void SetState(State state)
    {
        _battleFSM.SetState(state);
    }

    public void OnUpdate()
    {
        _battleFSM.OnUpdate();
    }
}
