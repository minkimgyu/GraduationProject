using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class JumpState : State
{
    PlayerController storedPlayer;

    public JumpState(PlayerController player)
    {
        storedPlayer = player;
    }

    public override void OnStateCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Ground")
        {
            storedPlayer.MovementFSM.RevertToPreviousState(); // ���� ���·� ������
        }
    }

    public override void OnStateEnter()
    {
        storedPlayer.MovementComponent.Jump();
    }

    public override void OnStateFixedUpdate()
    {
        storedPlayer.MovementComponent.Move();
    }

    public override void OnStateLateUpdate()
    {
        storedPlayer.ViewComponent.ResetCamera();
    }

    public override void OnStateUpdate()
    {
        storedPlayer.MovementComponent.ResetDirection();
        storedPlayer.ViewComponent.ResetView();

        storedPlayer.MovementComponent.RaiseDisplacementEvent(); // �̵� ���� ���� ������ ���̸� �̹�Ʈ�� �Ѱ���
    }
}
