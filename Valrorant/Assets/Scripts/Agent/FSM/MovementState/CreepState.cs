using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class CreepState : IState
{
    PlayerController storedPlayer;

    public CreepState(PlayerController player)
    {
        storedPlayer = player;
    }

    

    public void OnStateCollisionEnter(Collision collision)
    {
    }

    public void OnStateEnter()
    {
    }

    public void OnStateExit()
    {
    }

    public void OnStateFixedUpdate()
    {
        storedPlayer.MovementComponent.Move(true);
    }

    public void OnStateLateUpdate()
    {
        storedPlayer.ViewComponent.ResetCamera();
    }

    public void OnStateUpdate()
    {
        storedPlayer.MovementComponent.ResetDirection();
        storedPlayer.ViewComponent.ResetView();

        storedPlayer.MovementComponent.RaiseDisplacementEvent(); // �̵� ���� ���� ������ ���̸� �̹�Ʈ�� �Ѱ���
    }

    public void CheckStateChange()
    {
        if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
        {
            storedPlayer.MovementFSM.SetState(PlayerController.MovementState.Stop);
        }

        if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) && Input.GetKeyUp(KeyCode.LeftAlt))
        {
            storedPlayer.MovementFSM.SetState(PlayerController.MovementState.Walk);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            storedPlayer.MovementFSM.SetState(PlayerController.MovementState.Jump);
        }
    }

    public void OnStateTriggerEnter(Collider collider) { }

    public void OnStateTriggerExit(Collider collider) { }
}
