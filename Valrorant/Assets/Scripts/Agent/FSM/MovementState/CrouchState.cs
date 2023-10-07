using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class CrouchState : IState
{
    Player storedPlayer;

    public CrouchState(Player player)
    {
        storedPlayer = player;
    }

    public void OnStateEnter()
    {
        storedPlayer.MovementComponent.Crouch();
        storedPlayer.MovementFSM.RevertToPreviousState(); // 이전 상태로 돌려줌
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
