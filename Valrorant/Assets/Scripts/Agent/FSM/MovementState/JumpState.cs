using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class JumpState : IState
{
    Player storedPlayer;

    public JumpState(Player player)
    {
        storedPlayer = player;
    }

    public void OnStateEnter()
    {
        storedPlayer.MovementComponent.Jump();
        storedPlayer.MovementFSM.RevertToPreviousState(); // ���� ���·� ������
    }

    public void OnStateExit()
    {
    }

    public void OnStateFixedUpdate()
    {
    }

    public void OnStateLateUpdate()
    {
    }

    public void OnStateUpdate()
    {
    }
}
