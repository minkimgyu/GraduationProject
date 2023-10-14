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
            storedPlayer.MovementFSM.RevertToPreviousState(); // ���� ���·� ������
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

        // Ctrl�� �� ��� ��ũ���� ���
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            storedPlayer.MovementComponent.Crouch(false);
        }
    }

    public void CheckStateChange()
    {
    }
}
