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

    public void OnStateCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Ground")
        {
            storedPlayer.MovementFSM.RevertToPreviousState(); // 이전 상태로 돌려줌
        }
    }

    public void OnStateEnter()
    {
        storedPlayer.MovementComponent.Jump();
    }

    public void OnStateExit()
    {
    }

    public void OnStateFixedUpdate()
    {
        storedPlayer.MovementComponent.Move();
    }

    public void OnStateLateUpdate()
    {
        storedPlayer.ViewComponent.ResetCamera();
    }

    public void OnStateUpdate()
    {
        storedPlayer.MovementComponent.ResetDirection();
        storedPlayer.ViewComponent.ResetView();

        storedPlayer.MovementComponent.RaiseDisplacementEvent(); // 이동 값에 따른 백터의 길이를 이밴트로 넘겨줌

        // Ctrl를 땔 경우 웅크리기 취소
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            storedPlayer.MovementComponent.ChangePosture(false);
        }
    }

    public void CheckStateChange()
    {
    }
}
