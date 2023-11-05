using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class StopState : IState
{
    PlayerController storedPlayer;

    public StopState(PlayerController player)
    {
        storedPlayer = player;
    }

    public void CheckStateChange()
    {
        if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) && Input.GetKeyDown(KeyCode.LeftAlt))
        {
            storedPlayer.MovementFSM.SetState(PlayerController.MovementState.Creep);
        }

        if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0))
        {
            storedPlayer.MovementFSM.SetState(PlayerController.MovementState.Walk);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            storedPlayer.MovementFSM.SetState(PlayerController.MovementState.Jump);
        }
    }

    public void OnStateEnter()
    {
    }

    public void OnStateExit()
    {
    }

    public void OnStateFixedUpdate()
    {
    }

    public void OnStateLateUpdate()
    {
        storedPlayer.ViewComponent.ResetCamera();
    }

    public void OnStateUpdate()
    {
        storedPlayer.ViewComponent.ResetView();
        storedPlayer.MovementComponent.RaiseDisplacementEvent(); // �̵� ���� ���� ������ ���̸� �̹�Ʈ�� �Ѱ���
    }

    public void OnStateCollisionEnter(Collision collision)
    {
    }
}
